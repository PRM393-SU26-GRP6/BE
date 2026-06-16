using CourtManager.Domain.Entities;

namespace CourtManager.Application.Interfaces;

/// <summary>
/// Repository interface for WalletTransaction entity with specific queries.
/// </summary>
public interface IWalletTransactionRepository : IRepository<WalletTransaction>
{
    Task<IEnumerable<WalletTransaction>> GetByOwnerIdAsync(Guid ownerId, CancellationToken cancellationToken = default);
    Task<IEnumerable<WalletTransaction>> GetByOwnerIdPagedAsync(Guid ownerId, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<int> GetCountByOwnerIdAsync(Guid ownerId, CancellationToken cancellationToken = default);
}
