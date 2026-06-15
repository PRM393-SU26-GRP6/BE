using MediatR;

namespace CourtManager.Application.Features.TimeSlots.Commands;

/// <summary>
/// Lock a time slot for checkout.
/// 
/// If SlotId is provided, locks the existing slot.
/// Otherwise, creates a new TimeSlot with the specified field/date/time if it doesn't exist,
/// then locks it.
/// </summary>
public record LockSlotCommand(
    Guid? SlotId,
    Guid FieldId,
    DateOnly SelectedDate,
    TimeOnly StartTime,
    TimeOnly EndTime,
    Guid UserId
) : IRequest<LockSlotResult>;

public class LockSlotResult
{
    public Guid SlotId { get; set; }
    public DateTime LockedUntil { get; set; }
    public bool IsNewSlot { get; set; }
}
