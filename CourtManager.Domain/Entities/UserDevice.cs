namespace CourtManager.Domain.Entities;

/// <summary>
/// Represents a registered device of a user for push notifications (FCM).
/// One user can have multiple devices.
/// </summary>
public class UserDevice
{
    public Guid DeviceId { get; set; }
    public Guid UserId { get; set; }
    public string FcmToken { get; set; } = string.Empty;
    public string DeviceType { get; set; } = string.Empty; // "android" | "ios"
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Navigation property
    public User? User { get; set; }
}
