using CourtManager.Domain.Enums;

namespace CourtManager.Domain.Entities;

/// <summary>
/// Represents a bookable time slot for a football field.
/// </summary>
public class TimeSlot
{
    public Guid SlotId { get; set; }
    public Guid FieldId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public decimal Price { get; set; }
    public SlotStatus SlotStatus { get; set; } = SlotStatus.Available;
    public DateTime? LockedUntil { get; set; }
    public Guid? LockedBy { get; set; } // User holding the lock; null when available
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; }

    // Navigation properties
    public FootballField? Field { get; set; }
    public User? LockedByUser { get; set; }
    public ICollection<BookingItem> BookingItems { get; set; } = [];
}
