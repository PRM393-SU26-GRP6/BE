using CourtManager.Domain.Entities;

namespace CourtManager.Domain.Interfaces;

public interface IVenueAmenityRepository
{
    Task<VenueAmenity?> GetAsync(Guid venueId, Guid amenityId, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid venueId, Guid amenityId, CancellationToken cancellationToken = default);
    Task AddAsync(VenueAmenity venueAmenity, CancellationToken cancellationToken = default);
    Task DeleteAsync(VenueAmenity venueAmenity, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}

