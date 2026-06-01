using Microsoft.EntityFrameworkCore;
using CourtManager.Domain.Entities;
using CourtManager.Domain.Interfaces;

namespace CourtManager.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for Message entity.
/// Inherits from base Repository and implements IMessageRepository.
/// </summary>
public class MessageRepository : Repository<Message>, IMessageRepository
{
    public MessageRepository(ApplicationDbContext context) : base(context) { }

    public override async Task<Message?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(m => m.Sender)
            .FirstOrDefaultAsync(m => m.MessageId == id, cancellationToken);
    }

    public async Task<IEnumerable<Message>> GetMessagesByRoomIdAsync(
        Guid roomId,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(m => m.Sender)
            .Where(m => m.RoomId == roomId)
            .OrderByDescending(m => m.SentAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Message>> GetMessagesBeforeAsync(
        Guid roomId,
        DateTime? beforeSentAt,
        Guid? beforeMessageId,
        int limit,
        CancellationToken cancellationToken = default)
    {
        var effectiveBeforeSentAt = beforeSentAt;

        if (beforeMessageId.HasValue)
        {
            var cursorMessage = await _dbSet
                .Where(m => m.RoomId == roomId && m.MessageId == beforeMessageId.Value)
                .Select(m => new { m.SentAt })
                .FirstOrDefaultAsync(cancellationToken);

            if (cursorMessage != null)
            {
                effectiveBeforeSentAt = cursorMessage.SentAt;
            }
        }

        var query = _dbSet
            .Include(m => m.Sender)
            .Where(m => m.RoomId == roomId);

        if (effectiveBeforeSentAt.HasValue)
        {
            query = query.Where(m => m.SentAt < effectiveBeforeSentAt.Value);
        }

        return await query
            .OrderByDescending(m => m.SentAt)
            .Take(limit)
            .ToListAsync(cancellationToken);
    }

    public async Task<Message?> GetLastMessageAsync(
        Guid roomId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(m => m.RoomId == roomId)
            .OrderByDescending(m => m.SentAt)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<int> GetMessageCountAsync(
        Guid roomId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(m => m.RoomId == roomId)
            .CountAsync(cancellationToken);
    }

    public async Task MarkRoomMessagesAsReadAsync(Guid roomId, Guid readerId, CancellationToken cancellationToken = default)
    {
        var messages = await _dbSet
            .Where(m => m.RoomId == roomId && m.SenderId != readerId && m.ReadAt == null)
            .ToListAsync(cancellationToken);

        foreach (var message in messages)
        {
            message.ReadAt = DateTime.UtcNow;
        }
    }

    public async Task<int> GetUnreadCountForRoomAsync(Guid roomId, Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .CountAsync(m => m.RoomId == roomId && m.SenderId != userId && m.ReadAt == null, cancellationToken);
    }
}
