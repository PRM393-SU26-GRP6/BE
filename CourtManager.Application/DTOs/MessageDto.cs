namespace CourtManager.Application.DTOs;

/// <summary>
/// Data Transfer Object for Message entity.
/// Represents a single message sent in a chat room.
/// </summary>
public class MessageDto
{
    /// <summary>
    /// Unique identifier for the message.
    /// </summary>
    public Guid MessageId { get; set; }

    /// <summary>
    /// Foreign key referencing the chat room.
    /// </summary>
    public Guid RoomId { get; set; }

    /// <summary>
    /// Foreign key referencing the sender user.
    /// </summary>
    public Guid SenderId { get; set; }

    /// <summary>
    /// Display name of the message sender (optional).
    /// </summary>
    public string? SenderName { get; set; }

    /// <summary>
    /// The text content of the message.
    /// </summary>
    public string MessageText { get; set; } = string.Empty;

    public bool IsRead { get; set; }

    /// <summary>
    /// Timestamp when the message was sent.
    /// </summary>
    public DateTime SentAt { get; set; }
}
