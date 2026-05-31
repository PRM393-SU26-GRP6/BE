using CourtManager.Domain.Entities;

namespace CourtManager.Domain.Interfaces;

public interface IDiscountRepository : IRepository<Discount>
{
    Task<Discount?> GetByCodeAsync(string code, Guid? fieldId = null, Guid? ownerId = null, CancellationToken cancellationToken = default);
    Task<IEnumerable<Discount>> GetByOwnerAsync(Guid ownerId, CancellationToken cancellationToken = default);
}
