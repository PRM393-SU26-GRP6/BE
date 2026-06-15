namespace CourtManager.Application.DTOs;

/// <summary>
/// Request body for POST /api/v1/slots/lock.
/// Can lock by SlotId (existing slot) or by field/date/time (creates new slot if needed).
/// </summary>
public class LockSlotRequestDto
{
    /// <summary>
    /// Existing slot ID (optional).
    /// If provided, locks that specific slot.
    /// </summary>
    public Guid? SlotId { get; set; }

    /// <summary>
    /// Field ID (required if SlotId is null).
    /// </summary>
    public Guid FieldId { get; set; }

    /// <summary>
    /// Date for the slot (required if SlotId is null).
    /// </summary>
    public DateOnly SelectedDate { get; set; }

    /// <summary>
    /// Start time (required if SlotId is null).
    /// </summary>
    public TimeOnly StartTime { get; set; }

    /// <summary>
    /// End time (required if SlotId is null).
    /// </summary>
    public TimeOnly EndTime { get; set; }
}
