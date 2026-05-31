using MediatR;

namespace CourtManager.Application.Features.Bookings.Commands;

/// <summary>
/// Command to lock a time slot during payment process.
/// Changes slot status from "Available" to "Locked".
/// Implements CQRS Command pattern.
/// </summary>
public class LockTimeSlotCommand : IRequest<bool>
{
    /// <summary>
    /// The ID of the time slot to lock.
    /// </summary>
    public Guid SlotId { get; set; }

    /// <summary>
    /// The booking ID this lock is associated with (for reference).
    /// </summary>
    public Guid BookingId { get; set; }

    /// <summary>
    /// The ID of the user locking the slot.
    /// </summary>
    public Guid UserId { get; set; }

    public LockTimeSlotCommand(Guid slotId, Guid bookingId, Guid userId)
    {
        SlotId = slotId;
        BookingId = bookingId;
        UserId = userId;
    }
}
