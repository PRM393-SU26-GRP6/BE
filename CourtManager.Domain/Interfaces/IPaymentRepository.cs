using CourtManager.Domain.Entities;

namespace CourtManager.Domain.Interfaces;

/// <summary>
/// Repository interface for Payment entity with specific queries.
/// </summary>
public interface IPaymentRepository : IRepository<Payment>
{
    Task<Payment?> GetByBookingIdAsync(Guid bookingId, CancellationToken cancellationToken = default);
    Task<Payment?> GetByTransactionIdAsync(string transactionId, CancellationToken cancellationToken = default);
    Task<Payment?> GetByGatewayReferenceAsync(string gateway, string referenceCode, CancellationToken cancellationToken = default);
    Task<Payment?> GetByGatewayTransactionIdAsync(string gateway, string gatewayTransactionId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Payment>> GetPaymentsByBookingIdAsync(Guid bookingId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Payment>> GetPaymentHistoryForUserAsync(Guid userId, int pageNumber, int pageSize, CancellationToken cancellationToken = default);
}
