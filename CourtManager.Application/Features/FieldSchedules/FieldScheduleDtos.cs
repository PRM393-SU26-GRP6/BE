namespace CourtManager.Application.Features.FieldSchedules;

/// <summary>
/// One row in the weekly schedule for a field. DayOfWeek: 0=Sun..6=Sat.
/// </summary>
public class FieldScheduleDto
{
    public Guid ScheduleId { get; set; }
    public Guid FieldId { get; set; }
    public int DayOfWeek { get; set; }
    public string OpenTime { get; set; } = "06:00";
    public string CloseTime { get; set; } = "23:00";
    public int SlotDurationMinutes { get; set; } = 60;
    public bool IsActive { get; set; } = true;
}

/// <summary>
/// Body for PUT /api/v1/owner/fields/{id}/schedule.
/// Owner sends exactly 7 rows (one per DayOfWeek 0..6). If a row is omitted the field is closed that day.
/// </summary>
public class UpsertFieldScheduleDto
{
    public List<FieldScheduleRowDto> Rows { get; set; } = new();
}

public class FieldScheduleRowDto
{
    public int DayOfWeek { get; set; }
    public string OpenTime { get; set; } = "06:00";
    public string CloseTime { get; set; } = "23:00";
    public int SlotDurationMinutes { get; set; } = 60;
    public bool IsActive { get; set; } = true;
}

/// <summary>
/// Response item for GET /api/v1/fields/{id}/slots?date=YYYY-MM-DD.
/// Represents a persisted TimeSlot row available for the requested field and date.
/// </summary>
public class SlotForDateDto
{
    public DateTime StartTime { get; set; }   // full UTC DateTime for convenience
    public DateTime EndTime { get; set; }     // full UTC DateTime
    public string StartTimeOfDay { get; set; } = ""; // "HH:mm" — for UI clock display
    public string EndTimeOfDay { get; set; } = "";   // "HH:mm"
    public decimal Price { get; set; }
    public string SlotStatus { get; set; } = "Available"; // Available / Locked / Booked
    public Guid SlotId { get; set; }           // identity of the persisted TimeSlot row
}

/// <summary>
/// Response item for GET /api/v1/fields/{id}/week-schedule.
/// One entry per day-of-week (Mon-Fri). Customer-facing: shows open/closed and operating hours.
/// </summary>
public class FieldWeekScheduleDto
{
    /// <summary>ISO day-of-week: 0=Sunday .. 6=Saturday.</summary>
    public int DayOfWeek { get; set; }
    public string DayName { get; set; } = "";
    public bool IsOpen { get; set; }
    public string? OpenTime { get; set; }
    public string? CloseTime { get; set; }
    public int SlotDurationMinutes { get; set; }
}

/// <summary>
/// Response for GET /api/v1/fields/{id}/week-schedule.
/// Returns Mon-Fri rows (days 1-5) suitable for a booking calendar.
/// </summary>
public class FieldWeekScheduleResponseDto
{
    public Guid FieldId { get; set; }
    public List<FieldWeekScheduleDto> Days { get; set; } = new();
}
