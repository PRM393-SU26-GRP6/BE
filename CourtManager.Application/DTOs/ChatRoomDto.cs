namespace CourtManager.Application.DTOs;

/// <summary>
/// Data Transfer Object for ChatRoom entity.
/// Represents a chat room between a customer and a field host.
/// </summary>
public class ChatRoomDto
{
    /// <summary>
    /// Unique identifier for the chat room.
    /// </summary>
    public Guid RoomId { get; set; }

    /// <summary>
    /// Foreign key referencing the customer user.
    /// </summary>
    public Guid CustomerId { get; set; }

    /// <summary>
    /// Foreign key referencing the field host/owner user.
    /// </summary>
    public Guid HostId { get; set; }

    /// <summary>
    /// Timestamp when the chat room was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Display name of the customer (optional).
    /// </summary>
    public string? CustomerName { get; set; }

    /// <summary>
    /// Display name of the host (optional).
    /// </summary>
    public string? HostName { get; set; }

    /// <summary>
    /// Preview of the last message sent in this room (optional).
    /// </summary>
    public string? LastMessagePreview { get; set; }

    /// <summary>
    /// Timestamp of the last message (optional).
    /// </summary>
    public DateTime? LastMessageTime { get; set; }
}
