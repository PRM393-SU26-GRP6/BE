namespace CourtManager.Application.DTOs;

/// <summary>
/// Data Transfer Object for TimeSlot entity.
/// Represents a bookable time slot for a football field.
/// </summary>
public class TimeSlotDto
{
    /// <summary>
    /// Unique identifier for the time slot.
    /// </summary>
    public Guid SlotId { get; set; }

    /// <summary>
    /// Foreign key referencing the football field.
    /// </summary>
    public Guid FieldId { get; set; }

    /// <summary>
    /// Start time of the slot.
    /// </summary>
    public TimeOnly StartTime { get; set; }

    /// <summary>
    /// End time of the slot.
    /// </summary>
    public TimeOnly EndTime { get; set; }

    /// <summary>
    /// The specific date this slot is booked for.
    /// </summary>
    public DateOnly SelectedDate { get; set; }

    public decimal Price { get; set; }

    /// <summary>
    /// Current status of the slot (Available, Locked, Booked, etc.).
    /// </summary>
    public string SlotStatus { get; set; } = string.Empty;

    /// <summary>
    /// Timestamp when the slot was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Timestamp when the slot was last updated (if applicable).
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
}
