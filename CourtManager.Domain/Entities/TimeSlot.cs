using System.ComponentModel.DataAnnotations;
using CourtManager.Domain.Enums;

namespace CourtManager.Domain.Entities;

/// <summary>
/// Represents a bookable time slot for a football field.
/// </summary>
public class TimeSlot
{
    public Guid SlotId { get; set; }
    public Guid FieldId { get; set; }

    /// <summary>
    /// Start time of the slot (time of day only, e.g., 09:00).
    /// </summary>
    public TimeOnly StartTime { get; set; }

    /// <summary>
    /// End time of the slot (time of day only, e.g., 10:00).
    /// </summary>
    public TimeOnly EndTime { get; set; }

    /// <summary>
    /// The specific date this slot is booked for.
    /// </summary>
    public DateOnly SelectedDate { get; set; }

    public decimal Price { get; set; }
    public SlotStatus SlotStatus { get; set; } = SlotStatus.Available;
    public DateTime? LockedUntil { get; set; }
    public Guid? LockedBy { get; set; } // User holding the lock; null when available
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; }

    /// <summary>
    /// Concurrency token for optimistic concurrency control.
    /// Prevents race conditions in slot booking.
    /// </summary>
    [ConcurrencyCheck]
    public uint RowVersion { get; set; } = 1;

    // Navigation properties
    public FootballField? Field { get; set; }
    public User? LockedByUser { get; set; }
    public ICollection<BookingItem> BookingItems { get; set; } = [];
}
