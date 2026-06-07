using CourtManager.Domain.Entities;

namespace CourtManager.Application.Interfaces;

/// <summary>
/// Repository interface for TimeSlot entity operations.
/// </summary>
public interface ITimeSlotRepository : IRepository<TimeSlot>
{
    /// <summary>
    /// Gets available time slots for a specific field on a given date.
    /// </summary>
    Task<IEnumerable<TimeSlot>> GetAvailableSlotsAsync(
        Guid fieldId,
        DateTime date,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all time slots for a specific field.
    /// </summary>
    Task<IEnumerable<TimeSlot>> GetSlotsByFieldIdAsync(
        Guid fieldId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates the status of a specific time slot.
    /// </summary>
    Task UpdateSlotStatusAsync(
        Guid slotId,
        string status,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all currently locked time slots (awaiting payment).
    /// </summary>
    Task<IEnumerable<TimeSlot>> GetLockedSlotsAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets time slots that have expired their lock timeout.
    /// </summary>
    Task<IEnumerable<TimeSlot>> GetLockedSlotsExpiredAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Batch updates slot status for multiple slots.
    /// </summary>
    Task BatchUpdateSlotStatusAsync(
        IEnumerable<Guid> slotIds,
        string status,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a time slot by ID with Field and Venue eagerly loaded.
    /// Used to avoid navigation null issues in booking logic.
    /// </summary>
    Task<TimeSlot?> GetByIdWithFieldVenueAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Atomically locks a slot if it is available.
    /// Uses optimistic concurrency to prevent double-booking.
    /// Returns true if lock acquired, false if slot already booked/locked.
    /// </summary>
    Task<bool> TryLockSlotAtomicAsync(
        Guid slotId,
        Guid userId,
        CancellationToken cancellationToken = default);
}
