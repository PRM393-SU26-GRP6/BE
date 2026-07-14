using CourtManager.Application.Exceptions;
using CourtManager.Domain.Entities;
using CourtManager.Application.Interfaces;
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
        // AcceptBookingAtomicAsync handles validation, updating the booking status to Accepted,
        // marking the TimeSlots as Booked, and rejecting any overlapping Pending/Deposited bookings
        // all within a single database transaction.
        return await _bookingRepository.AcceptBookingAtomicAsync(request.BookingId, request.OwnerId, cancellationToken);
    }
}
