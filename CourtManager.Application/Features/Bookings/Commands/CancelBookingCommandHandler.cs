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
    private readonly IWalletTransactionRepository _walletTransactionRepository;

    public CancelBookingCommandHandler(
        IBookingRepository bookingRepository,
        ITimeSlotRepository timeSlotRepository,
        INotificationRepository notificationRepository,
        IUserRepository userRepository,
        IPaymentRepository paymentRepository,
        IWalletTransactionRepository walletTransactionRepository)
    {
        _bookingRepository = bookingRepository;
        _timeSlotRepository = timeSlotRepository;
        _notificationRepository = notificationRepository;
        _userRepository = userRepository;
        _paymentRepository = paymentRepository;
        _walletTransactionRepository = walletTransactionRepository;
    }

    /// <summary>
    /// Handles the CancelBookingCommand.
    /// Verifies booking exists, is cancellable, updates status to Cancelled,
    /// and reverts associated time slots to Available.
    /// </summary>
    public async Task<bool> Handle(CancelBookingCommand request, CancellationToken cancellationToken)
    {
        // Fetch the booking
        var booking = await _bookingRepository.GetByIdAsync(request.BookingId, cancellationToken);
        if (booking == null)
            throw new NotFoundException(nameof(Booking), request.BookingId);

        if (!request.IsOwnerOrAdmin && booking.UserId != request.UserId)
            throw new ValidationException("Only the booking customer can cancel this booking.");

        // Verify booking is in a cancellable status (Pending or Confirmed)
        if (booking.BookingStatus != BookingStatus.Pending && booking.BookingStatus != BookingStatus.Accepted)
            throw new ValidationException(
                $"Cannot cancel booking. Current status is '{booking.BookingStatus}'. Only 'Pending' or 'Accepted' bookings can be cancelled.");

        // Update booking status to Cancelled and store cancellation reason
        booking.BookingStatus = BookingStatus.Cancelled;
        if (!string.IsNullOrEmpty(request.CancellationReason))
        {
            booking.Note = $"Cancelled: {request.CancellationReason}";
        }
        booking.UpdatedAt = DateTime.UtcNow;

        // Revert time slots to Available status
        if (booking.BookingItems != null && booking.BookingItems.Any())
        {
            var slotIds = booking.BookingItems.Select(bi => bi.SlotId).ToList();
            await _timeSlotRepository.BatchUpdateSlotStatusAsync(slotIds, "Available", cancellationToken);
        }

        var bookingItems = booking.BookingItems ?? [];
        var ownerId = bookingItems
            .Select(i => i.Slot?.Field?.Venue?.OwnerId)
            .FirstOrDefault(id => id.HasValue && id.Value != Guid.Empty);

        // Handle wallet refund if booking has been deposited
        if (ownerId.HasValue && booking.BookingStatus == BookingStatus.Accepted)
        {
            // Get successful payments for this booking
            var payments = await _paymentRepository.GetPaymentsByBookingIdAsync(booking.Id, cancellationToken);
            var successfulPayments = payments.Where(p => p.PaymentStatus == PaymentStatus.Success).ToList();
            
            if (successfulPayments.Any())
            {
                // Calculate refund amount (90% of successful payments - owner's share)
                const decimal commissionRate = 0.10m;
                decimal totalRefund = successfulPayments.Sum(p => p.Amount * (1 - commissionRate));
                
                // Get owner info
                var owner = await _userRepository.GetByIdWithWalletAsync(ownerId.Value, cancellationToken);
                if (owner != null)
                {
                    // Check if owner has sufficient balance
                    if (owner.WalletBalance >= totalRefund)
                    {
                        // Deduct from owner's wallet (refund to customer, so owner loses the amount they received)
                        await _userRepository.UpdateWalletBalanceAsync(ownerId.Value, -totalRefund, cancellationToken);
                        
                        // Create wallet transaction record
                        var walletTransaction = new WalletTransaction
                        {
                            Id = Guid.NewGuid(),
                            OwnerId = ownerId.Value,
                            Type = WalletTransactionType.Refund,
                            Amount = totalRefund,
                            Description = $"Refund from cancelled booking {booking.Id}",
                            RelatedBookingId = booking.Id,
                            CreatedAt = DateTime.UtcNow
                        };
                        await _walletTransactionRepository.AddAsync(walletTransaction, cancellationToken);
                    }
                    else
                    {
                        // Log insufficient balance for admin review
                        // In production, you'd want to create an alert or log entry here
                        // For now, we'll proceed with cancellation but the refund won't be processed
                    }
                }
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

        // Save changes
        await _bookingRepository.UpdateAsync(booking, cancellationToken);
        await _bookingRepository.SaveChangesAsync(cancellationToken);
        await _timeSlotRepository.SaveChangesAsync(cancellationToken);

        return true;
    }
}
