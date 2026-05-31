using CourtManager.Application.Exceptions;
using CourtManager.Domain.Entities;
using CourtManager.Domain.Interfaces;
using MediatR;

namespace CourtManager.Application.Features.Bookings.Commands;

/// <summary>
/// Handler for AcceptBookingCommand.
/// Implements the business logic for confirming a pending booking.
/// </summary>
public class AcceptBookingCommandHandler : IRequestHandler<AcceptBookingCommand, bool>
{
    private readonly IBookingRepository _bookingRepository;

    public AcceptBookingCommandHandler(IBookingRepository bookingRepository)
    {
        _bookingRepository = bookingRepository;
    }

    /// <summary>
    /// Handles the AcceptBookingCommand.
    /// Verifies booking exists and is in Pending status, then updates to Confirmed.
    /// </summary>
    public async Task<bool> Handle(AcceptBookingCommand request, CancellationToken cancellationToken)
    {
        // Fetch the booking
        var booking = await _bookingRepository.GetByIdAsync(request.BookingId, cancellationToken);
        if (booking == null)
            throw new NotFoundException(nameof(Booking), request.BookingId);

        if (request.OwnerId != Guid.Empty && !booking.BookingItems.Any(i => i.Slot?.Field?.Venue?.OwnerId == request.OwnerId))
            throw new ValidationException("Only the owner of the booked venue can accept this booking.");

        // Verify booking is in Pending status
        if (booking.BookingStatus != CourtManager.Domain.Enums.BookingStatus.Pending)
            throw new ValidationException(
                $"Cannot accept booking. Current status is '{booking.BookingStatus}'. Only 'Pending' bookings can be accepted.");

        // Update booking status to Confirmed
        booking.BookingStatus = CourtManager.Domain.Enums.BookingStatus.Accepted; // Updated from "Confirmed" to match the ER enum
        booking.UpdatedAt = DateTime.UtcNow;

        // Save changes
        await _bookingRepository.UpdateAsync(booking, cancellationToken);
        await _bookingRepository.SaveChangesAsync(cancellationToken);

        return true;
    }
}
