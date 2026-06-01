using CourtManager.Domain.Entities;

namespace CourtManager.Domain.Interfaces;

/// <summary>
/// Repository interface for Message entity operations.
/// </summary>
public interface IMessageRepository : IRepository<Message>
{
    /// <summary>
    /// Gets messages in a chat room with pagination.
    /// </summary>
    Task<IEnumerable<Message>> GetMessagesByRoomIdAsync(
        Guid roomId,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the last message sent in a chat room.
    /// </summary>
    Task<Message?> GetLastMessageAsync(
        Guid roomId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets total message count for a chat room.
    /// </summary>
    Task<int> GetMessageCountAsync(
        Guid roomId,
        CancellationToken cancellationToken = default);

    Task MarkRoomMessagesAsReadAsync(
        Guid roomId,
        Guid readerId,
        CancellationToken cancellationToken = default);

    Task<int> GetUnreadCountForRoomAsync(
        Guid roomId,
        Guid userId,
        CancellationToken cancellationToken = default);
}
