using CourtManager.Domain.Entities;
using CourtManager.Domain.Enums;

namespace CourtManager.Application.Interfaces;

/// <summary>
/// Repository interface for WithdrawalRequest entity with specific queries.
/// </summary>
public interface IWithdrawalRepository : IRepository<WithdrawalRequest>
{
    Task<WithdrawalRequest?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<WithdrawalRequest?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<WithdrawalRequest>> GetByOwnerIdAsync(Guid ownerId, CancellationToken cancellationToken = default);
    Task<IEnumerable<WithdrawalRequest>> GetAllPendingAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<WithdrawalRequest>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<WithdrawalRequest>> GetByStatusAsync(WithdrawalStatus status, CancellationToken cancellationToken = default);
    Task<bool> HasPendingRequestAsync(Guid ownerId, CancellationToken cancellationToken = default);
}
