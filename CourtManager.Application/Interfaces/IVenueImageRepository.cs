using CourtManager.Domain.Entities;

namespace CourtManager.Application.Interfaces;

/// <summary>
/// Repository interface for VenueImage entity operations.
/// </summary>
public interface IVenueImageRepository : IRepository<VenueImage>
{
    /// <summary>
    /// Gets all images for a specific venue.
    /// </summary>
    Task<IEnumerable<VenueImage>> GetImagesByVenueIdAsync(
        Guid venueId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes all images associated with a venue.
    /// </summary>
    Task DeleteByVenueIdAsync(
        Guid venueId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds multiple images at once.
    /// </summary>
    Task AddMultipleAsync(
        IEnumerable<VenueImage> images,
        CancellationToken cancellationToken = default);
}
