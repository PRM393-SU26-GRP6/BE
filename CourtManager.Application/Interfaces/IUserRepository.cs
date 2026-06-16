using CourtManager.Domain.Entities;

namespace CourtManager.Application.Interfaces;

/// <summary>
/// Repository interface for User entity with specific queries.
/// </summary>
public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(string email, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Updates the wallet balance for a user using atomic operations to prevent race conditions.
    /// </summary>
    Task UpdateWalletBalanceAsync(Guid userId, decimal amount, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets a user with wallet information for balance checks.
    /// </summary>
    Task<User?> GetByIdWithWalletAsync(Guid userId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets all owners with their wallet balances.
    /// </summary>
    Task<IEnumerable<User>> GetOwnersWithWalletsAsync(CancellationToken cancellationToken = default);
}
