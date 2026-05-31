using MediatR;

namespace CourtManager.Application.Features.Bookings.Commands;

/// <summary>
/// Command to reject a pending booking.
/// Changes booking status from "Pending" to "Rejected".
/// Implements CQRS Command pattern.
/// </summary>
public class RejectBookingCommand : IRequest<bool>
{
    /// <summary>
    /// The ID of the booking to reject.
    /// </summary>
    public Guid BookingId { get; set; }

    /// <summary>
    /// Optional reason for rejection.
    /// </summary>
    public string? RejectionReason { get; set; }
    public Guid OwnerId { get; set; }

    public RejectBookingCommand(Guid bookingId, string? rejectionReason = null, Guid ownerId = default)
    {
        BookingId = bookingId;
        RejectionReason = rejectionReason;
        OwnerId = ownerId;
    }
}
