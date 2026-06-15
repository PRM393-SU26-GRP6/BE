using AutoMapper;
using CourtManager.Application.DTOs;
using CourtManager.Application.Exceptions;
using CourtManager.Domain.Entities;
using CourtManager.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CourtManager.Application.Features.Bookings.Commands;

/// <summary>
/// Handler for CreateBookingCommand.
/// Implements the business logic for creating a new booking.
/// </summary>
public class CreateBookingCommandHandler : IRequestHandler<CreateBookingCommand, BookingDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IFootballFieldRepository _fieldRepository;
    private readonly IBookingRepository _bookingRepository;
    private readonly ITimeSlotRepository _timeSlotRepository;
    private readonly IDiscountRepository _discountRepository;
    private readonly INotificationRepository _notificationRepository;
    private readonly IMapper _mapper;

    public CreateBookingCommandHandler(
        IUserRepository userRepository,
        IFootballFieldRepository fieldRepository,
        IBookingRepository bookingRepository,
        ITimeSlotRepository timeSlotRepository,
        IDiscountRepository discountRepository,
        INotificationRepository notificationRepository,
        IMapper mapper)
    {
        _userRepository = userRepository;
        _fieldRepository = fieldRepository;
        _bookingRepository = bookingRepository;
        _timeSlotRepository = timeSlotRepository;
        _discountRepository = discountRepository;
        _notificationRepository = notificationRepository;
        _mapper = mapper;
    }

    /// <summary>
    /// Handles the CreateBookingCommand.
    /// Validates user and field existence, checks field availability,
    /// calculates total price, and creates the booking.
    /// </summary>
    public async Task<BookingDto> Handle(CreateBookingCommand request, CancellationToken cancellationToken)
    {
        var newlyLockedSlotIds = new List<Guid>();

        try
        {
            // Validate user exists
            var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
            if (user == null)
                throw new NotFoundException(nameof(User), request.UserId);

            var slots = new List<TimeSlot>();
            var now = DateTime.UtcNow;

            if (request.SlotIds.Length > 0)
            {
                foreach (var slotId in request.SlotIds.Distinct())
                {
                    var slot = await _timeSlotRepository.GetByIdWithFieldVenueAsync(slotId, cancellationToken);
                    if (slot == null)
                        throw new NotFoundException(nameof(TimeSlot), slotId);

                    if (slot.SelectedDate < DateOnly.FromDateTime(now) ||
                        (slot.SelectedDate == DateOnly.FromDateTime(now) && slot.StartTime < TimeOnly.FromDateTime(now)))
                        throw new ValidationException("Cannot book a time slot in the past.");

                    var isExpiredLock = slot.SlotStatus == CourtManager.Domain.Enums.SlotStatus.Locked
                        && slot.LockedUntil.HasValue
                        && slot.LockedUntil.Value <= now;

                    var isLockedByCurrentUser = slot.SlotStatus == CourtManager.Domain.Enums.SlotStatus.Locked
                        && slot.LockedBy == request.UserId
                        && slot.LockedUntil.HasValue
                        && slot.LockedUntil.Value > now;

                    if (slot.SlotStatus == CourtManager.Domain.Enums.SlotStatus.Booked)
                        throw new ValidationException($"Slot {slotId} is already booked.");

                    if (!isLockedByCurrentUser)
                    {
                        var lockAcquired = await _timeSlotRepository.TryLockSlotAtomicAsync(slotId, request.UserId, cancellationToken);
                        if (!lockAcquired)
                            throw new ValidationException($"Slot {slotId} is not available.");

                        newlyLockedSlotIds.Add(slotId);

                        slot = await _timeSlotRepository.GetByIdWithFieldVenueAsync(slotId, cancellationToken);
                        if (slot == null)
                            throw new NotFoundException(nameof(TimeSlot), slotId);
                    }
                    else if (isExpiredLock)
                    {
                        throw new ValidationException($"Slot {slotId} is not available.");
                    }

                    slots.Add(slot);
                }
            }

            if (slots.Count == 0)
                throw new ValidationException("A booking must contain at least one slot.");

            if (slots.Any(s => s.Field == null || s.Field.Venue == null))
                throw new ValidationException("Slot metadata is incomplete. Please try again.");

            var venueIds = slots.Select(s => s.Field!.VenueId).Distinct().ToList();
            if (venueIds.Count != 1)
                throw new ValidationException("All slots in one booking must belong to the same venue.");

            var totalBeforeDiscount = slots.Sum(s => s.Price);
            var discountAmount = 0m;
            Discount? discount = null;

            if (!string.IsNullOrWhiteSpace(request.DiscountCode))
            {
                var fieldId = slots.First().FieldId;
                var ownerId = slots.First().Field!.Venue!.OwnerId;
                discount = await _discountRepository.GetByCodeAsync(request.DiscountCode, fieldId, ownerId, cancellationToken);
                if (discount == null || !discount.IsActive || discount.StartDate > now || discount.EndDate < now)
                    throw new ValidationException("Discount code is invalid or expired.");

                if (discount.UsageLimit > 0 && discount.UsedCount >= discount.UsageLimit)
                    throw new ValidationException("Discount code has reached its usage limit.");

                if (discount.MinBookingAmount > totalBeforeDiscount)
                    throw new ValidationException("Booking total does not meet the discount minimum amount.");

                discountAmount = discount.DiscountType == CourtManager.Domain.Enums.DiscountType.Percentage
                    ? totalBeforeDiscount * discount.Value / 100
                    : discount.Value;

                if (discount.MaxDiscountAmount > 0)
                    discountAmount = Math.Min(discountAmount, discount.MaxDiscountAmount);

                discountAmount = Math.Min(discountAmount, totalBeforeDiscount);
            }

            var totalPrice = totalBeforeDiscount - discountAmount;

            var booking = new Booking
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
                TotalPrice = totalPrice,
                DepositAmount = Math.Round(totalPrice * 0.5m, 2),
                BookingStatus = CourtManager.Domain.Enums.BookingStatus.Pending,
                Note = request.Note,
                CreatedAt = now,
                BookingItems = slots.Select(slot => new BookingItem
                {
                    BookingItemId = Guid.NewGuid(),
                    SlotId = slot.SlotId,
                    Price = slot.Price,
                    CreatedAt = now
                }).ToList()
            };

            if (discount != null && discountAmount > 0)
            {
                var incremented = await _discountRepository.TryIncrementUsedCountAsync(discount.DiscountId, cancellationToken);
                if (!incremented)
                    throw new ValidationException("Discount code has reached its usage limit.");

                booking.BookingDiscounts.Add(new BookingDiscount
                {
                    BookingId = booking.Id,
                    DiscountId = discount.DiscountId,
                    DiscountAmount = discountAmount
                });
            }

            var notificationOwnerId = slots
                .Select(s => s.Field!.Venue!.OwnerId)
                .FirstOrDefault(id => id != Guid.Empty);

            if (notificationOwnerId != Guid.Empty)
            {
                await _notificationRepository.AddAsync(new Notification
                {
                    NotificationId = Guid.NewGuid(),
                    SenderId = request.UserId,
                    Title = "New booking request",
                    Message = $"A new booking request {booking.Id} is waiting for approval.",
                    Type = CourtManager.Domain.Enums.NotificationType.Booking,
                    RefId = booking.Id.ToString(),
                    CreatedAt = now,
                    NotificationRecipients =
                    [
                        new NotificationRecipient
                        {
                            RecipientId = Guid.NewGuid(),
                            UserId = notificationOwnerId
                        }
                    ]
                }, cancellationToken);
            }

            var createdBooking = await _bookingRepository.AddAsync(booking, cancellationToken);
            await _bookingRepository.SaveChangesAsync(cancellationToken);

            var loaded = await _bookingRepository.GetByIdAsync(createdBooking.Id, cancellationToken) ?? createdBooking;
            return _mapper.Map<BookingDto>(loaded);
        }
        catch (DbUpdateConcurrencyException)
        {
            await ReleaseNewlyLockedSlotsAsync(newlyLockedSlotIds, request.UserId, cancellationToken);
            throw new ValidationException("One or more selected slots were booked by another user. Please refresh and try again.");
        }
        catch
        {
            await ReleaseNewlyLockedSlotsAsync(newlyLockedSlotIds, request.UserId, cancellationToken);
            throw;
        }
    }

    private async Task ReleaseNewlyLockedSlotsAsync(IEnumerable<Guid> slotIds, Guid userId, CancellationToken cancellationToken)
    {
        foreach (var slotId in slotIds.Distinct())
        {
            var slot = await _timeSlotRepository.GetByIdAsync(slotId, cancellationToken);
            if (slot == null)
                continue;

            var lockStillOwnedByCurrentUser = slot.SlotStatus == CourtManager.Domain.Enums.SlotStatus.Locked
                && slot.LockedBy == userId;

            if (!lockStillOwnedByCurrentUser)
                continue;

            slot.SlotStatus = CourtManager.Domain.Enums.SlotStatus.Available;
            slot.LockedBy = null;
            slot.LockedUntil = null;
            slot.UpdatedAt = DateTime.UtcNow;

            await _timeSlotRepository.UpdateAsync(slot, cancellationToken);
        }

        if (slotIds.Any())
            await _timeSlotRepository.SaveChangesAsync(cancellationToken);
    }
}
