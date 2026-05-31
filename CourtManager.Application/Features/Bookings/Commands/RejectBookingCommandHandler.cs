using CourtManager.Application.Exceptions;
using CourtManager.Domain.Entities;
using CourtManager.Domain.Interfaces;
using MediatR;

namespace CourtManager.Application.Features.Bookings.Commands;

/// <summary>
/// Handler for RejectBookingCommand.
/// Implements the business logic for rejecting a pending booking.
/// </summary>
public class RejectBookingCommandHandler : IRequestHandler<RejectBookingCommand, bool>
{
    private readonly IBookingRepository _bookingRepository;
    private readonly ITimeSlotRepository _timeSlotRepository;

    public RejectBookingCommandHandler(IBookingRepository bookingRepository, ITimeSlotRepository timeSlotRepository)
    {
        _bookingRepository = bookingRepository;
        _timeSlotRepository = timeSlotRepository;
    }

    /// <summary>
    /// Handles the RejectBookingCommand.
    /// Verifies booking exists and is in Pending status, then updates to Rejected.
    /// </summary>
    public async Task<bool> Handle(RejectBookingCommand request, CancellationToken cancellationToken)
    {
        // Fetch the booking
        var booking = await _bookingRepository.GetByIdAsync(request.BookingId, cancellationToken);
        if (booking == null)
            throw new NotFoundException(nameof(Booking), request.BookingId);

        if (request.OwnerId != Guid.Empty && !booking.BookingItems.Any(i => i.Slot?.Field?.Venue?.OwnerId == request.OwnerId))
            throw new ValidationException("Only the owner of the booked venue can reject this booking.");

        // Verify booking is in Pending status
        if (booking.BookingStatus != CourtManager.Domain.Enums.BookingStatus.Pending)
            throw new ValidationException(
                $"Cannot reject booking. Current status is '{booking.BookingStatus}'. Only 'Pending' bookings can be rejected.");

        // Update booking status to Rejected and store rejection reason in Note
        booking.BookingStatus = CourtManager.Domain.Enums.BookingStatus.Rejected;
        if (!string.IsNullOrEmpty(request.RejectionReason))
        {
            booking.Note = $"Rejected: {request.RejectionReason}";
        }
        booking.UpdatedAt = DateTime.UtcNow;

        if (booking.BookingItems.Any())
        {
            await _timeSlotRepository.BatchUpdateSlotStatusAsync(booking.BookingItems.Select(i => i.SlotId), "Available", cancellationToken);
        }

        // Save changes
        await _bookingRepository.UpdateAsync(booking, cancellationToken);
        await _bookingRepository.SaveChangesAsync(cancellationToken);

        return true;
    }
}
