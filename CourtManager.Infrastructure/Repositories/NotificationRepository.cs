using Microsoft.EntityFrameworkCore;
using CourtManager.Domain.Entities;
using CourtManager.Domain.Interfaces;

namespace CourtManager.Infrastructure.Repositories;

public class NotificationRepository : Repository<Notification>, INotificationRepository
{
    public NotificationRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IEnumerable<Notification>> GetNotificationsByUserIdAsync(Guid userId, bool unreadOnly = false, CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .Include(n => n.NotificationRecipients)
            .Where(n => n.NotificationRecipients.Any(r => r.UserId == userId));

        if (unreadOnly)
        {
            query = query.Where(n => n.NotificationRecipients.Any(r => r.UserId == userId && r.ReadAt == null));
        }

        return await query
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Notification>> GetNotificationsByUserIdPaginatedAsync(Guid userId, int pageNumber, int pageSize, bool unreadOnly = false, CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .Include(n => n.NotificationRecipients)
            .Where(n => n.NotificationRecipients.Any(r => r.UserId == userId));

        if (unreadOnly)
        {
            query = query.Where(n => n.NotificationRecipients.Any(r => r.UserId == userId && r.ReadAt == null));
        }

        return await query
            .OrderByDescending(n => n.CreatedAt)
            .Skip((Math.Max(pageNumber, 1) - 1) * Math.Max(pageSize, 1))
            .Take(Math.Max(pageSize, 1))
            .ToListAsync(cancellationToken);
    }

    public async Task MarkAsReadAsync(Guid notificationId, Guid userId, CancellationToken cancellationToken = default)
    {
        var recipient = await _context.NotificationRecipients
            .FirstOrDefaultAsync(r => r.NotificationId == notificationId && r.UserId == userId, cancellationToken);
        if (recipient != null && recipient.ReadAt == null)
        {
            recipient.ReadAt = DateTime.UtcNow;
        }
    }

    public async Task MarkAllAsReadAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var recipients = await _context.NotificationRecipients
            .Where(r => r.UserId == userId && r.ReadAt == null)
            .ToListAsync(cancellationToken);

        foreach (var recipient in recipients)
        {
            recipient.ReadAt = DateTime.UtcNow;
        }
    }

    public async Task<int> GetUnreadCountAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.NotificationRecipients
            .CountAsync(r => r.UserId == userId && r.ReadAt == null, cancellationToken);
    }
}
