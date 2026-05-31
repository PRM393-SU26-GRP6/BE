namespace CourtManager.Domain.Entities;

/// <summary>
/// Represents a recipient of a notification.
/// </summary>
public class NotificationRecipient
{
    public Guid RecipientId { get; set; }
    public Guid NotificationId { get; set; }
    public Guid UserId { get; set; }
    public DateTime? ReadAt { get; set; } // null = unread

    // Navigation properties
    public Notification? Notification { get; set; }
    public User? User { get; set; }
}
