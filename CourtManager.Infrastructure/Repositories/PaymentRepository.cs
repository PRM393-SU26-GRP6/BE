using Microsoft.EntityFrameworkCore;
using CourtManager.Domain.Entities;
using CourtManager.Domain.Interfaces;

namespace CourtManager.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for Payment entity.
/// Inherits from base Repository and implements IPaymentRepository.
/// </summary>
public class PaymentRepository : Repository<Payment>, IPaymentRepository
{
    public PaymentRepository(ApplicationDbContext context) : base(context) { }

    public override async Task<Payment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(p => p.Booking)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<Payment?> GetByBookingIdAsync(Guid bookingId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(p => p.Booking)
            .FirstOrDefaultAsync(p => p.BookingId == bookingId, cancellationToken);
    }

    public async Task<Payment?> GetByTransactionIdAsync(string transactionCode, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(p => p.Booking)
                .ThenInclude(b => b!.BookingItems)
                    .ThenInclude(i => i.Slot)
            .FirstOrDefaultAsync(p => p.TransactionCode == transactionCode, cancellationToken);
    }

    public async Task<Payment?> GetByGatewayReferenceAsync(string gateway, string referenceCode, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(p => p.Booking)
            .FirstOrDefaultAsync(p =>
                p.Gateway == gateway &&
                p.GatewayReferenceCode == referenceCode,
                cancellationToken);
    }

    public async Task<Payment?> GetByGatewayTransactionIdAsync(string gateway, string gatewayTransactionId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(p => p.Booking)
            .FirstOrDefaultAsync(p =>
                p.Gateway == gateway &&
                p.GatewayTransactionId == gatewayTransactionId,
                cancellationToken);
    }

    public async Task<IEnumerable<Payment>> GetPaymentsByBookingIdAsync(Guid bookingId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(p => p.Booking)
            .Where(p => p.BookingId == bookingId)
            .OrderByDescending(p => p.PaidAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Payment>> GetPaymentHistoryForUserAsync(Guid userId, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(p => p.Booking)
            .Where(p => p.Booking != null && p.Booking.UserId == userId)
            .OrderByDescending(p => p.PaidAt)
            .Skip((Math.Max(pageNumber, 1) - 1) * Math.Max(pageSize, 1))
            .Take(Math.Max(pageSize, 1))
            .ToListAsync(cancellationToken);
    }
}
