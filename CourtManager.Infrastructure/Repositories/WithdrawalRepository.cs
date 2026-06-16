using Microsoft.EntityFrameworkCore;
using CourtManager.Domain.Entities;
using CourtManager.Domain.Enums;
using CourtManager.Application.Interfaces;
using CourtManager.Infrastructure;

namespace CourtManager.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for WithdrawalRequest entity.
/// </summary>
public class WithdrawalRepository : Repository<WithdrawalRequest>, IWithdrawalRepository
{
    public WithdrawalRepository(ApplicationDbContext context) : base(context) { }

    public async Task<WithdrawalRequest?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FirstOrDefaultAsync(w => w.Id == id, cancellationToken);
    }

    public async Task<WithdrawalRequest?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(w => w.Owner)
            .Include(w => w.ApprovedByAdmin)
            .FirstOrDefaultAsync(w => w.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<WithdrawalRequest>> GetByOwnerIdAsync(Guid ownerId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(w => w.OwnerId == ownerId)
            .OrderByDescending(w => w.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<WithdrawalRequest>> GetAllPendingAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(w => w.Owner)
            .Where(w => w.Status == WithdrawalStatus.Pending)
            .OrderByDescending(w => w.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<WithdrawalRequest>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(w => w.Owner)
            .OrderByDescending(w => w.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<WithdrawalRequest>> GetByStatusAsync(WithdrawalStatus status, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(w => w.Owner)
            .Where(w => w.Status == status)
            .OrderByDescending(w => w.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> HasPendingRequestAsync(Guid ownerId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AnyAsync(w => w.OwnerId == ownerId && w.Status == WithdrawalStatus.Pending, cancellationToken);
    }
}
