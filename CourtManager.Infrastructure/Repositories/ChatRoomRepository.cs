using Microsoft.EntityFrameworkCore;
using CourtManager.Domain.Entities;
using CourtManager.Domain.Interfaces;

namespace CourtManager.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for ChatRoom entity.
/// Inherits from base Repository and implements IChatRoomRepository.
/// </summary>
public class ChatRoomRepository : Repository<ChatRoom>, IChatRoomRepository
{
    public ChatRoomRepository(ApplicationDbContext context) : base(context) { }

    public async Task<ChatRoom> GetOrCreateChatRoomAsync(
        Guid customerId,
        Guid hostId,
        CancellationToken cancellationToken = default)
    {
        var existingRoom = await GetChatRoomByUsersAsync(customerId, hostId, cancellationToken);
        if (existingRoom != null)
        {
            return existingRoom;
        }

        var newRoom = new ChatRoom
        {
            RoomId = Guid.NewGuid(),
            CustomerId = customerId,
            HostId = hostId,
            CreatedAt = DateTime.UtcNow
        };

        await AddAsync(newRoom, cancellationToken);
        return newRoom;
    }

    public async Task<ChatRoom?> GetChatRoomByUsersAsync(
        Guid userId1,
        Guid userId2,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(r => r.Customer)
            .Include(r => r.Host)
            .FirstOrDefaultAsync(r =>
                (r.CustomerId == userId1 && r.HostId == userId2) ||
                (r.CustomerId == userId2 && r.HostId == userId1),
                cancellationToken);
    }

    public async Task<IEnumerable<ChatRoom>> GetChatRoomsForUserAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(r => r.Customer)
            .Include(r => r.Host)
            .Include(r => r.Messages)
            .Where(r => r.CustomerId == userId || r.HostId == userId)
            .OrderByDescending(r => r.LastMessageAt ?? r.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<ChatRoom>> GetChatRoomsForUserPaginatedAsync(
        Guid userId,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(r => r.Customer)
            .Include(r => r.Host)
            .Include(r => r.Messages)
            .Where(r => r.CustomerId == userId || r.HostId == userId)
            .OrderByDescending(r => r.LastMessageAt ?? r.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public override async Task<ChatRoom?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(r => r.Customer)
            .Include(r => r.Host)
            .Include(r => r.Messages)
            .FirstOrDefaultAsync(r => r.RoomId == id, cancellationToken);
    }
}
