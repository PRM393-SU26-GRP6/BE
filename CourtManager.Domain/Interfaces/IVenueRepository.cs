using CourtManager.Domain.Entities;

namespace CourtManager.Domain.Interfaces;

public interface IVenueRepository : IRepository<Venue>
{
    // We will add custom methods here if needed later (e.g. specialized GetVenuesAsync)
    Task<IEnumerable<Venue>> GetVenuesAsync(
        string? q, 
        decimal? priceMin, 
        decimal? priceMax, 
        double? minRating, 
        int skip, 
        int take, 
        CancellationToken cancellationToken = default);

    Task<int> GetTotalCountAsync(
        string? q, 
        decimal? priceMin, 
        decimal? priceMax, 
        double? minRating, 
        CancellationToken cancellationToken = default);

    Task<IEnumerable<(Venue Venue, double Distance)>> GetNearbyVenuesAsync(
        double latitude,
        double longitude,
        double radiusInKm,
        CancellationToken cancellationToken = default);

    Task<Venue?> GetVenueByIdAsync(Guid venueId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Amenity>> GetVenueAmenitiesAsync(Guid venueId, CancellationToken cancellationToken = default);

    // Owner-scoped listing (includes inactive venues; owner sees all of their own)
    Task<IEnumerable<Venue>> GetOwnerVenuesAsync(
        Guid ownerId,
        bool? isActive,
        int skip,
        int take,
        CancellationToken cancellationToken = default);

    Task<int> GetOwnerVenuesCountAsync(
        Guid ownerId,
        bool? isActive,
        CancellationToken cancellationToken = default);
}
