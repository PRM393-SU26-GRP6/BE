using CourtManager.Application.Exceptions;
using CourtManager.Domain.Entities;
using CourtManager.Domain.Enums;
using CourtManager.Application.Interfaces;
using MediatR;

namespace CourtManager.Application.Features.Bookings.Commands;

/// <summary>
/// Handler for CancelBookingCommand.
/// Implements the business logic for cancelling a booking and unlocking reserved time slots.
/// </summary>
public class CancelBookingCommandHandler : IRequestHandler<CancelBookingCommand, bool>
{
    private readonly IBookingRepository _bookingRepository;
    private readonly ITimeSlotRepository _timeSlotRepository;
    private readonly INotificationRepository _notificationRepository;
    private readonly IUserRepository _userRepository;
    private readonly IPaymentRepository _paymentRepository;

    public CancelBookingCommandHandler(
        IBookingRepository bookingRepository,
        ITimeSlotRepository timeSlotRepository,
        INotificationRepository notificationRepository,
        IUserRepository userRepository,
        IPaymentRepository paymentRepository)
    {
        _bookingRepository = bookingRepository;
        _timeSlotRepository = timeSlotRepository;
        _notificationRepository = notificationRepository;
        _userRepository = userRepository;
        _paymentRepository = paymentRepository;
    }

    public async Task<bool> Handle(CancelBookingCommand request, CancellationToken cancellationToken)
    {
        var booking = await _bookingRepository.GetByIdAsync(request.BookingId, cancellationToken);
        if (booking == null)
            throw new NotFoundException(nameof(Booking), request.BookingId);

        if (!request.IsOwnerOrAdmin && booking.UserId != request.UserId)
            throw new ValidationException("Only the booking customer can cancel this booking.");

        if (booking.BookingStatus != BookingStatus.Pending && booking.BookingStatus != BookingStatus.Accepted)
            throw new ValidationException(
                $"Cannot cancel booking. Current status is '{booking.BookingStatus}'. Only 'Pending' or 'Accepted' bookings can be cancelled.");

        booking.BookingStatus = BookingStatus.Cancelled;
        if (!string.IsNullOrEmpty(request.CancellationReason))
        {
            booking.Note = $"Cancelled: {request.CancellationReason}";
        }
        booking.UpdatedAt = DateTime.UtcNow;

        if (booking.BookingItems != null && booking.BookingItems.Any())
        {
            var slotIds = booking.BookingItems.Select(bi => bi.SlotId).ToList();
            await _timeSlotRepository.BatchUpdateSlotStatusAsync(slotIds, "Available", cancellationToken);
        }

        var bookingItems = booking.BookingItems ?? [];
        var ownerId = bookingItems
            .Select(i => i.Slot?.Field?.Venue?.OwnerId)
            .FirstOrDefault(id => id.HasValue && id.Value != Guid.Empty);

        if (ownerId.HasValue && booking.BookingStatus == BookingStatus.Accepted)
        {
            var payments = await _paymentRepository.GetPaymentsByBookingIdAsync(booking.Id, cancellationToken);
            var successfulPayments = payments.Where(p => p.PaymentStatus == PaymentStatus.Success).ToList();
            
            if (successfulPayments.Any())
            {
                // Refund logic is now handled manually by Admin offline
            }
        }

        if (ownerId.HasValue)
        {
            var notification = new Notification
            {
                NotificationId = Guid.NewGuid(),
                SenderId = request.UserId,
                Title = "Booking cancelled",
                Message = $"Booking {booking.Id} has been cancelled.",
                Type = NotificationType.Booking,
                RefId = booking.Id.ToString(),
                CreatedAt = DateTime.UtcNow,
                Recipients =
                [
                    new NotificationRecipient
                    {
                        RecipientId = Guid.NewGuid(),
                        UserId = ownerId.Value
                    }
                ]
            };

            await _notificationRepository.AddAsync(notification, cancellationToken);
        }

        await _bookingRepository.SaveChangesAsync(cancellationToken);
        await _timeSlotRepository.SaveChangesAsync(cancellationToken);

        return true;
    }
}
