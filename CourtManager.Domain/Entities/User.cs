using Microsoft.AspNetCore.Identity;

namespace CourtManager.Domain.Entities;

/// <summary>
/// Represents a user in the system (user, owner, or admin).
/// </summary>
public class User : IdentityUser<Guid>
{
    public string FullName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? AvatarUrl { get; set; }
    public int LoyaltyPoints { get; set; } = 0;
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiryTime { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsActive { get; set; }
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; }

    // OTP fields for email verification
    public string? OtpCode { get; set; }
    public DateTime? OtpExpiryTime { get; set; }
    public int OtpAttempts { get; set; } = 0;

    // Wallet fields
    public decimal WalletBalance { get; set; } = 0m;

    // Navigation properties
    public ICollection<Booking> Bookings { get; set; } = [];
    public virtual ICollection<UserRole> UserRoles { get; set; } = [];
    public ICollection<Venue> OwnedVenues { get; set; } = [];
    public ICollection<ChatRoom> CustomerChatRooms { get; set; } = [];
    public ICollection<ChatRoom> HostChatRooms { get; set; } = [];
    public ICollection<Message> SentMessages { get; set; } = [];
    public ICollection<Notification> Notifications { get; set; } = [];
    public ICollection<NotificationRecipient> NotificationRecipients { get; set; } = [];
    public ICollection<Review> Reviews { get; set; } = [];
    public ICollection<Discount> CreatedDiscounts { get; set; } = [];
    public ICollection<UserDevice> Devices { get; set; } = [];
    
    // Wallet navigation properties
    public ICollection<WalletTransaction> WalletTransactions { get; set; } = [];
    public ICollection<WithdrawalRequest> WithdrawalRequests { get; set; } = [];
}
