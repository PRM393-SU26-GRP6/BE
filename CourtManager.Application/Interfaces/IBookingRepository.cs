using CourtManager.Domain.Entities;

namespace CourtManager.Application.Interfaces;

/// <summary>
/// Repository interface for Booking entity with specific queries.
/// </summary>
public interface IBookingRepository : IRepository<Booking>
{
    Task<IEnumerable<Booking>> GetBookingsByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Booking>> GetBookingsByCourtIdAsync(Guid courtId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Booking>> GetPendingBookingsForOwnerAsync(Guid ownerId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Booking>> GetBookingsForOwnerAsync(Guid ownerId, CancellationToken cancellationToken = default);
    Task<bool> IsCourtAvailableAsync(Guid courtId, DateTime startTime, DateTime endTime, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns true if the venue has any booking with status Pending, Accepted, or Deposited.
    /// Used to block venue deactivation while bookings are still in-flight.
    /// </summary>
    Task<bool> HasActiveBookingsForVenueAsync(Guid venueId, CancellationToken cancellationToken = default);

    Task<bool> IsBookingValidForReviewAsync(Guid bookingId, Guid venueId, Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a booking by ID with all navigation properties eagerly loaded (Items, Slots, Field, Venue, Payments, etc.)
    /// </summary>
    Task<Booking?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
}
