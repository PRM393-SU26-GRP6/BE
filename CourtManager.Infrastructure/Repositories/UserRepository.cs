using Microsoft.EntityFrameworkCore;
using CourtManager.Domain.Entities;
using CourtManager.Application.Interfaces;

namespace CourtManager.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for User entity.
/// Inherits from base Repository and implements IUserRepository.
/// </summary>
public class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(ApplicationDbContext context) : base(context) { }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
    }

    public async Task<bool> ExistsAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AnyAsync(u => u.Email == email, cancellationToken);
    }

    public async Task UpdateWalletBalanceAsync(Guid userId, decimal amount, CancellationToken cancellationToken = default)
    {
        await _dbSet
            .Where(u => u.Id == userId)
            .ExecuteUpdateAsync(u => u.SetProperty(x => x.WalletBalance, x => x.WalletBalance + amount), cancellationToken);
    }

    public async Task<User?> GetByIdWithWalletAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
    }

    public async Task<IEnumerable<User>> GetOwnersWithWalletsAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(u => u.UserRoles.Any(ur => ur.Role != null && ur.Role.Name == "Owner"))
            .ToListAsync(cancellationToken);
    }
}
