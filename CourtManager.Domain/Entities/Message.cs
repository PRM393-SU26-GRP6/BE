namespace CourtManager.Domain.Entities;

/// <summary>
/// Represents a message sent within a chat room.
/// </summary>
public class Message
{
    public Guid MessageId { get; set; }
    public Guid RoomId { get; set; }
    public Guid SenderId { get; set; }
    public string MessageText { get; set; } = string.Empty;
    public DateTime? ReadAt { get; set; } // null = unread
    public DateTime SentAt { get; set; }
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; }

    // Navigation properties
    public ChatRoom? Room { get; set; }
    public User? Sender { get; set; }
}
