using CourtManager.Domain.Enums;

namespace CourtManager.Domain.Entities;

/// <summary>
/// Represents a football field available for booking.
/// </summary>
public class FootballField
{
    public Guid Id { get; set; }
    public Guid VenueId { get; set; }
    public string FieldName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public FieldType FieldType { get; set; }
    public decimal PricePerHour { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsActive { get; set; }
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; }

    // Navigation properties
    public Venue? Venue { get; set; }
    public ICollection<Booking> Bookings { get; set; } = [];
    public ICollection<TimeSlot> TimeSlots { get; set; } = [];
    public ICollection<FieldSchedule> FieldSchedules { get; set; } = [];
}
