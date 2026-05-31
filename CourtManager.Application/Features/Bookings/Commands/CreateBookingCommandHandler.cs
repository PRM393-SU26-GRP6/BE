using AutoMapper;
using CourtManager.Application.DTOs;
using CourtManager.Application.Exceptions;
using CourtManager.Domain.Entities;
using CourtManager.Domain.Interfaces;
using MediatR;

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
        // Validate user exists
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user == null)
            throw new NotFoundException(nameof(User), request.UserId);

        var slots = new List<TimeSlot>();

        if (request.SlotIds.Length > 0)
        {
            foreach (var slotId in request.SlotIds.Distinct())
            {
                var slot = await _timeSlotRepository.GetByIdAsync(slotId, cancellationToken);
                if (slot == null)
                    throw new NotFoundException(nameof(TimeSlot), slotId);

                if (slot.StartTime <= DateTime.UtcNow)
                    throw new ValidationException("Cannot book a time slot in the past.");

                if (slot.SlotStatus == CourtManager.Domain.Enums.SlotStatus.Locked && slot.LockedUntil.HasValue && slot.LockedUntil.Value <= DateTime.UtcNow)
                {
                    slot.SlotStatus = CourtManager.Domain.Enums.SlotStatus.Available;
                    slot.LockedUntil = null;
                }

                if (slot.SlotStatus != CourtManager.Domain.Enums.SlotStatus.Available)
                {
                    bool isLockedByCurrentUser = slot.SlotStatus == CourtManager.Domain.Enums.SlotStatus.Locked && slot.LockedBy == request.UserId;
                    if (!isLockedByCurrentUser)
                        throw new ValidationException($"Slot {slotId} is not available.");
                }

                slots.Add(slot);
            }
        }
        else
        {
            // Backward-compatible path for older clients that post field/start/end.
            var field = await _fieldRepository.GetByIdAsync(request.FieldId, cancellationToken);
            if (field == null)
                throw new NotFoundException(nameof(FootballField), request.FieldId);

            var fieldSlots = await _timeSlotRepository.GetSlotsByFieldIdAsync(request.FieldId, cancellationToken);
            slots = fieldSlots
                .Where(s => s.StartTime == request.StartTime && s.EndTime == request.EndTime)
                .ToList();

            if (slots.Count == 0)
                throw new ValidationException("No matching time slot exists for the requested field/time.");
        }

        if (slots.Count == 0)
            throw new ValidationException("A booking must contain at least one slot.");

        var venueIds = slots.Select(s => s.Field?.VenueId).Distinct().ToList();
        if (venueIds.Count != 1)
            throw new ValidationException("All slots in one booking must belong to the same venue.");

        var totalBeforeDiscount = slots.Sum(s => s.Price);
        var discountAmount = 0m;
        Discount? discount = null;

        if (!string.IsNullOrWhiteSpace(request.DiscountCode))
        {
            var fieldId = slots.First().FieldId;
            var ownerId = slots.First().Field?.Venue?.OwnerId;
            discount = await _discountRepository.GetByCodeAsync(request.DiscountCode, fieldId, ownerId, cancellationToken);
            if (discount == null || !discount.IsActive || discount.StartDate > DateTime.UtcNow || discount.EndDate < DateTime.UtcNow)
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

        // Create booking entity
        var booking = new Booking
        {
            Id = Guid.NewGuid(),
            UserId = request.UserId,
            TotalPrice = totalPrice,
            DepositAmount = Math.Round(totalPrice * 0.5m, 2),
            BookingStatus = CourtManager.Domain.Enums.BookingStatus.Pending,
            Note = request.Note,
            CreatedAt = DateTime.UtcNow,
            BookingItems = slots.Select(slot => new BookingItem
            {
                BookingItemId = Guid.NewGuid(),
                SlotId = slot.SlotId,
                Price = slot.Price,
                CreatedAt = DateTime.UtcNow
            }).ToList()
        };

        if (discount != null && discountAmount > 0)
        {
            booking.BookingDiscounts.Add(new BookingDiscount
            {
                BookingId = booking.Id,
                DiscountId = discount.DiscountId,
                DiscountAmount = discountAmount
            });
            discount.UsedCount += 1;
            await _discountRepository.UpdateAsync(discount, cancellationToken);
        }

        foreach (var slot in slots)
        {
            slot.SlotStatus = CourtManager.Domain.Enums.SlotStatus.Locked;
            slot.LockedUntil = DateTime.UtcNow.AddMinutes(15);
            slot.UpdatedAt = DateTime.UtcNow;
        }

        var notificationOwnerId = slots
            .Select(s => s.Field?.Venue?.OwnerId)
            .FirstOrDefault(id => id.HasValue && id.Value != Guid.Empty);

        if (notificationOwnerId.HasValue)
        {
            await _notificationRepository.AddAsync(new Notification
            {
                NotificationId = Guid.NewGuid(),
                SenderId = request.UserId,
                Title = "New booking request",
                Message = $"A new booking request {booking.Id} is waiting for approval.",
                Type = CourtManager.Domain.Enums.NotificationType.Booking,
                RefId = booking.Id.ToString(),
                CreatedAt = DateTime.UtcNow,
                NotificationRecipients =
                [
                    new NotificationRecipient
                    {
                        RecipientId = Guid.NewGuid(),
                        UserId = notificationOwnerId.Value
                    }
                ]
            }, cancellationToken);
        }

        // Save booking
        var createdBooking = await _bookingRepository.AddAsync(booking, cancellationToken);
        await _bookingRepository.SaveChangesAsync(cancellationToken);

        var loaded = await _bookingRepository.GetByIdAsync(createdBooking.Id, cancellationToken) ?? createdBooking;
        return _mapper.Map<BookingDto>(loaded);
    }
}
