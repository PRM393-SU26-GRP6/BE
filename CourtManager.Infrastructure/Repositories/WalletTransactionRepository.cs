using Microsoft.EntityFrameworkCore;
using CourtManager.Domain.Entities;
using CourtManager.Application.Interfaces;
using CourtManager.Infrastructure;

namespace CourtManager.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for WalletTransaction entity.
/// </summary>
public class WalletTransactionRepository : Repository<WalletTransaction>, IWalletTransactionRepository
{
    public WalletTransactionRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IEnumerable<WalletTransaction>> GetByOwnerIdAsync(Guid ownerId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(w => w.OwnerId == ownerId)
            .OrderByDescending(w => w.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<WalletTransaction>> GetByOwnerIdPagedAsync(Guid ownerId, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(w => w.OwnerId == ownerId)
            .OrderByDescending(w => w.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> GetCountByOwnerIdAsync(Guid ownerId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .CountAsync(w => w.OwnerId == ownerId, cancellationToken);
    }
}
