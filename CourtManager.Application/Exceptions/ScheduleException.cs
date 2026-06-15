namespace CourtManager.Application.Exceptions;

/// <summary>
/// Thrown when a request requires a FieldSchedule row that does not exist
/// (e.g. Owner has not configured a schedule, or the field is closed on that day of week).
/// Mapped to HTTP 404 by the global exception handler.
/// </summary>
public class ScheduleException : Exception
{
    public ScheduleException(string message) : base(message) { }
}
