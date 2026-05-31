using CourtManager.Domain.Entities;

namespace CourtManager.Domain.Interfaces;

/// <summary>
/// Repository interface for Notification entity operations.
/// </summary>
public interface IReviewRepository : IRepository<Review>
{
    /// <summary>
    /// Gets reviews for a specific football field with pagination.
    /// </summary>
    Task<IEnumerable<Review>> GetReviewsByFieldIdAsync(
        Guid fieldId,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a specific user's review for a field (if exists).
    /// </summary>
    Task<Review?> GetUserReviewAsync(
        Guid userId,
        Guid fieldId,
        CancellationToken cancellationToken = default);

    Task<Review?> GetReviewByBookingIdAsync(Guid bookingId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets reviews, total count, and average rating for a venue.
    /// </summary>
    Task<(IEnumerable<Review> Reviews, int TotalCount, decimal AverageRating)> GetVenueReviewsAsync(
        Guid venueId,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);
}
