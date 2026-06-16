using CourtManager.Domain.Entities;

namespace CourtManager.Application.Interfaces;

/// <summary>
/// Repository for FieldSchedule entity. Exposes the minimum methods needed to:
///   - load a full week of schedule for a field (Owner view, frontend week UI)
///   - load the single schedule row for (FieldId, DayOfWeek) (used when generating slots for a specific date)
///   - replace all 7 rows for a field atomically (Owner upsert)
/// </summary>
public interface IFieldScheduleRepository
{
    /// <summary>Returns all 7 schedule rows for a field ordered by DayOfWeek (0..6).</summary>
    Task<IEnumerable<FieldSchedule>> GetByFieldIdAsync(Guid fieldId, CancellationToken cancellationToken = default);

    /// <summary>Returns the single row matching (fieldId, dayOfWeek) or null.</summary>
    Task<FieldSchedule?> GetForDayAsync(Guid fieldId, int dayOfWeek, CancellationToken cancellationToken = default);

    /// <summary>
    /// Replaces all schedule rows for the field with the supplied list. Used by Owner upsert.
    /// Inserts new rows, updates existing rows by (FieldId, DayOfWeek), deletes rows that are no longer in the list.
    /// Must run inside a single transaction.
    /// </summary>
    Task ReplaceAllForFieldAsync(Guid fieldId, IEnumerable<FieldSchedule> schedules, CancellationToken cancellationToken = default);
}
