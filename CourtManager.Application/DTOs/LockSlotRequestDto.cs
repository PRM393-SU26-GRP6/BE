namespace CourtManager.Application.DTOs;

/// <summary>
/// Request body for POST /api/v1/slots/lock.
/// Requires the ID of an existing persisted slot.
/// </summary>
public class LockSlotRequestDto
{
    /// <summary>
    /// Existing persisted slot ID to lock.
    /// </summary>
    public Guid SlotId { get; set; }
}
