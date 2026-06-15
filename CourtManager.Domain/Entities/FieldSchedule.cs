using CourtManager.Domain.Enums;

namespace CourtManager.Domain.Entities;

/// <summary>
/// Per-day-of-week operating schedule for a FootballField.
/// One row per (FieldId, DayOfWeek). Used to generate bookable slots at runtime
/// for any date in any week without re-seeding or migrating.
/// </summary>
public class FieldSchedule
{
    public Guid ScheduleId { get; set; }

    public Guid FieldId { get; set; }

    /// <summary>0 = Sunday, 1 = Monday, ..., 6 = Saturday. Matches System.DayOfWeek.</summary>
    public int DayOfWeek { get; set; }

    public TimeOnly OpenTime { get; set; }

    public TimeOnly CloseTime { get; set; }

    public int SlotDurationMinutes { get; set; } = 60;

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool IsDeleted { get; set; }

    // Navigation
    public FootballField? Field { get; set; }
}
