using CourtManager.Domain.Entities;

namespace CourtManager.Application.Interfaces;

/// <summary>
/// Repository interface for ChatRoom entity operations.
/// </summary>
public interface IChatRoomRepository : IRepository<ChatRoom>
{
    /// <summary>
    /// Gets or creates a chat room between two users.
    /// </summary>
    Task<ChatRoom> GetOrCreateChatRoomAsync(
        Guid customerId,
        Guid hostId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets an existing chat room between two users (if any).
    /// </summary>
    Task<ChatRoom?> GetChatRoomByUsersAsync(
        Guid userId1,
        Guid userId2,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all chat rooms where the user is either customer or host.
    /// </summary>
    Task<IEnumerable<ChatRoom>> GetChatRoomsForUserAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets paginated chat rooms for a user with pagination support.
    /// </summary>
    Task<IEnumerable<ChatRoom>> GetChatRoomsForUserPaginatedAsync(
        Guid userId,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);
}
