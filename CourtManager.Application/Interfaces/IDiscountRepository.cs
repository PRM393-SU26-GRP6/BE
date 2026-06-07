using CourtManager.Domain.Entities;

namespace CourtManager.Application.Interfaces;

public interface IDiscountRepository : IRepository<Discount>
{
    Task<Discount?> GetByCodeAsync(string code, Guid? fieldId = null, Guid? ownerId = null, CancellationToken cancellationToken = default);
    Task<IEnumerable<Discount>> GetByOwnerAsync(Guid ownerId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Atomically increments the UsedCount of a discount.
    /// Returns true if increment succeeded (within limits), false if limit reached.
    /// </summary>
    Task<bool> TryIncrementUsedCountAsync(Guid discountId, CancellationToken cancellationToken = default);
}
