using Microsoft.EntityFrameworkCore;
using CourtManager.Domain.Entities;
using CourtManager.Application.Interfaces;

namespace CourtManager.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for FootballField entity.
/// Inherits from base Repository and implements IFootballFieldRepository.
/// </summary>
public class FootballFieldRepository : Repository<FootballField>, IFootballFieldRepository
{
    public FootballFieldRepository(ApplicationDbContext context) : base(context) { }

    public override async Task<FootballField?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(f => f.Venue)
            .FirstOrDefaultAsync(f => f.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<FootballField>> GetAvailableFieldsAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(f => f.IsActive)
            .Include(f => f.Venue)
            .ToListAsync(cancellationToken);
    }

    public async Task<FootballField?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(f => f.Venue)
            .FirstOrDefaultAsync(f => f.FieldName == name, cancellationToken);
    }

    public async Task<IEnumerable<FootballField>> GetFieldsByVenueIdAsync(Guid venueId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(f => f.VenueId == venueId && !f.IsDeleted)
            .Include(f => f.Venue)
            .ToListAsync(cancellationToken);
    }
}
