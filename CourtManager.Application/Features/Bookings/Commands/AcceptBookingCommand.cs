using MediatR;

namespace CourtManager.Application.Features.Bookings.Commands;

/// <summary>
/// Command to accept/confirm a pending booking.
/// Changes booking status from "Pending" to "Confirmed".
/// Implements CQRS Command pattern.
/// </summary>
public class AcceptBookingCommand : IRequest<bool>
{
    /// <summary>
    /// The ID of the booking to accept.
    /// </summary>
    public Guid BookingId { get; set; }
    public Guid OwnerId { get; set; }

    public AcceptBookingCommand(Guid bookingId, Guid ownerId = default)
    {
        BookingId = bookingId;
        OwnerId = ownerId;
    }
}
