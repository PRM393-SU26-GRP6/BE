using MediatR;

namespace CourtManager.Application.Features.TimeSlots.Commands;

/// <summary>
/// Locks an existing persisted time slot for checkout.
/// </summary>
public record LockSlotCommand(Guid SlotId, Guid UserId) : IRequest<LockSlotResult>;

public class LockSlotResult
{
    public Guid SlotId { get; set; }
    public DateTime LockedUntil { get; set; }
    public bool IsNewSlot { get; set; }
}
