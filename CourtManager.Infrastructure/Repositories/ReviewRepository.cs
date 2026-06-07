using Microsoft.EntityFrameworkCore;
using CourtManager.Domain.Entities;
using CourtManager.Application.Interfaces;

namespace CourtManager.Infrastructure.Repositories;

public class ReviewRepository : Repository<Review>, IReviewRepository
{
    public ReviewRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IEnumerable<Review>> GetReviewsByFieldIdAsync(Guid fieldId, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(r => r.User)
            .Include(r => r.Booking)
            .Where(r => r.Booking != null && r.Booking.BookingItems.Any(bi => bi.Slot != null && bi.Slot.FieldId == fieldId) && !r.IsDeleted)
            .OrderByDescending(r => r.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<Review?> GetUserReviewAsync(Guid userId, Guid fieldId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(r => r.Booking)
            .FirstOrDefaultAsync(r => r.UserId == userId && r.Booking != null && r.Booking.BookingItems.Any(bi => bi.Slot != null && bi.Slot.FieldId == fieldId) && !r.IsDeleted, cancellationToken);
    }

    public async Task<IEnumerable<Review>> GetReviewsByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(r => r.Venue)
            .Where(r => r.UserId == userId && !r.IsDeleted)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<(IEnumerable<Review> Reviews, int TotalCount, decimal AverageRating)> GetVenueReviewsAsync(
        Guid venueId, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = _dbSet.Where(r => r.VenueId == venueId && !r.IsDeleted);

        var totalCount = await query.CountAsync(cancellationToken);
        decimal averageRating = 0;
        
        if (totalCount > 0)
        {
            // Cast to double for Average calculation, then cast back to decimal
            var avg = await query.AverageAsync(r => (double)r.Rating, cancellationToken);
            averageRating = (decimal)Math.Round(avg, 1);
        }

        var reviews = await query
            .Include(r => r.User)
            .OrderByDescending(r => r.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (reviews, totalCount, averageRating);
    }

    public async Task<Review?> GetReviewByBookingIdAsync(Guid bookingId, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FirstOrDefaultAsync(r => r.BookingId == bookingId && !r.IsDeleted, cancellationToken);
    }
}
