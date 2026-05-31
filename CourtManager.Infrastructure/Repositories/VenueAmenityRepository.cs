using CourtManager.Domain.Entities;
using CourtManager.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CourtManager.Infrastructure.Repositories;

public class VenueAmenityRepository : IVenueAmenityRepository
{
    private readonly ApplicationDbContext _context;

    public VenueAmenityRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<VenueAmenity?> GetAsync(Guid venueId, Guid amenityId, CancellationToken cancellationToken = default)
    {
        return await _context.VenueAmenities.FindAsync(new object[] { venueId, amenityId }, cancellationToken);
    }

    public async Task<bool> ExistsAsync(Guid venueId, Guid amenityId, CancellationToken cancellationToken = default)
    {
        return await _context.VenueAmenities.AnyAsync(
            va => va.VenueId == venueId && va.AmenityId == amenityId,
            cancellationToken);
    }

    public async Task AddAsync(VenueAmenity venueAmenity, CancellationToken cancellationToken = default)
    {
        await _context.VenueAmenities.AddAsync(venueAmenity, cancellationToken);
    }

    public Task DeleteAsync(VenueAmenity venueAmenity, CancellationToken cancellationToken = default)
    {
        _context.VenueAmenities.Remove(venueAmenity);
        return Task.CompletedTask;
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}

