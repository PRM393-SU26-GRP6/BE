using CourtManager.Domain.Entities;
using CourtManager.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CourtManager.Infrastructure.Repositories;

public class DiscountRepository : Repository<Discount>, IDiscountRepository
{
    public DiscountRepository(ApplicationDbContext context) : base(context) { }

    public async Task<Discount?> GetByCodeAsync(string code, Guid? fieldId = null, Guid? ownerId = null, CancellationToken cancellationToken = default)
    {
        var normalizedCode = code.Trim().ToUpperInvariant();
        var query = _dbSet
            .Include(d => d.Venue)
            .Where(d => d.Code.ToUpper() == normalizedCode);

        if (ownerId.HasValue)
        {
            query = query.Where(d => d.OwnerId == ownerId.Value);
        }

        if (fieldId.HasValue)
        {
            query = query.Where(d => d.VenueId == null || d.VenueId == fieldId.Value);
        }

        return await query
            .OrderByDescending(d => d.VenueId.HasValue)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IEnumerable<Discount>> GetByOwnerAsync(Guid ownerId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(d => d.OwnerId == ownerId)
            .OrderByDescending(d => d.CreatedAt)
            .ToListAsync(cancellationToken);
    }
}
