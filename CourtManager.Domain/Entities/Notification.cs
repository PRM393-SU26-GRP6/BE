using CourtManager.Domain.Enums;

namespace CourtManager.Domain.Entities;

/// <summary>
/// Represents a notification sent to a user.
/// </summary>
public class Notification
{
    public Guid NotificationId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public NotificationType Type { get; set; }
    public string RefId { get; set; } = string.Empty;
    public Guid SenderId { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; }

    // Navigation properties
    public ICollection<NotificationRecipient> Recipients { get; set; } = [];
}
