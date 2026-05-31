namespace CourtManager.Application.DTOs;

/// <summary>
/// Data Transfer Object for Notification entity.
/// Represents a notification sent to a user.
/// </summary>
public class NotificationDto
{
    /// <summary>
    /// Unique identifier for the notification.
    /// </summary>
    public Guid NotificationId { get; set; }

    /// <summary>
    /// Foreign key referencing the recipient user.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Title/subject of the notification.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Main content/body of the notification.
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Flag indicating whether the notification has been read.
    /// </summary>
    public bool IsRead { get; set; }

    public string Type { get; set; } = string.Empty;

    public string RefId { get; set; } = string.Empty;

    public DateTime? ReadAt { get; set; }

    /// <summary>
    /// Timestamp when the notification was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }
}
