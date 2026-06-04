using Microsoft.EntityFrameworkCore;
using CourtManager.Domain.Entities;
using CourtManager.Application.Interfaces;

namespace CourtManager.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for VenueImage entity.
/// Inherits from base Repository and implements IVenueImageRepository.
/// </summary>
public class VenueImageRepository : Repository<VenueImage>, IVenueImageRepository
{
    public VenueImageRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IEnumerable<VenueImage>> GetImagesByVenueIdAsync(
        Guid venueId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(i => i.VenueId == venueId)
            .ToListAsync(cancellationToken);
    }

    public async Task DeleteByVenueIdAsync(
        Guid venueId,
        CancellationToken cancellationToken = default)
    {
        var images = await _dbSet
            .Where(i => i.VenueId == venueId)
            .ToListAsync(cancellationToken);

        _dbSet.RemoveRange(images);
    }

    public async Task AddMultipleAsync(
        IEnumerable<VenueImage> images,
        CancellationToken cancellationToken = default)
    {
        await _dbSet.AddRangeAsync(images, cancellationToken);
    }
}
