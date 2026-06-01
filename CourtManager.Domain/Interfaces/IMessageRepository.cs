using CourtManager.Domain.Entities;

namespace CourtManager.Domain.Interfaces;

/// <summary>
/// Repository interface for Notification entity operations.
/// </summary>
public interface INotificationRepository : IRepository<Notification>
{
    /// <summary>
    /// Gets notifications for a specific user with optional filtering for unread only.
    /// </summary>
    Task<IEnumerable<Notification>> GetNotificationsByUserIdAsync(
        Guid userId,
        bool unreadOnly = false,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets paginated notifications for a user.
    /// </summary>
    Task<IEnumerable<Notification>> GetNotificationsByUserIdPaginatedAsync(
        Guid userId,
        int pageNumber,
        int pageSize,
        bool unreadOnly = false,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Marks a specific notification as read.
    /// </summary>
    Task MarkAsReadAsync(
        Guid notificationId,
        Guid userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Marks all notifications for a user as read.
    /// </summary>
    Task MarkAllAsReadAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets unread notification count for a user.
    /// </summary>
    Task<int> GetUnreadCountAsync(
        Guid userId,
        CancellationToken cancellationToken = default);
}
