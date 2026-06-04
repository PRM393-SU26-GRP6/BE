using CourtManager.APIs.Hubs;
using CourtManager.Application.DTOs;
using Microsoft.AspNetCore.SignalR;

namespace CourtManager.APIs.Services.Realtime;

public class RealtimeEventPublisher : IRealtimeEventPublisher
{
    private readonly IHubContext<ChatHub> _chatHubContext;
    private readonly IHubContext<NotificationHub> _notificationHubContext;

    public RealtimeEventPublisher(
        IHubContext<ChatHub> chatHubContext,
        IHubContext<NotificationHub> notificationHubContext)
    {
        _chatHubContext = chatHubContext;
        _notificationHubContext = notificationHubContext;
    }

    public Task PublishChatMessageCreatedAsync(MessageDto message, CancellationToken cancellationToken = default)
    {
        return _chatHubContext.Clients
            .Group(RealtimeConstants.Groups.ChatRoom(message.RoomId))
            .SendAsync(RealtimeConstants.Events.ChatMessageCreated, message, cancellationToken);
    }

    public async Task PublishChatRoomUpdatedAsync(ChatRoomDto room, CancellationToken cancellationToken = default)
    {
        var customerGroup = RealtimeConstants.Groups.User(room.CustomerId);
        var hostGroup = RealtimeConstants.Groups.User(room.HostId);

        if (customerGroup == hostGroup)
        {
            await _chatHubContext.Clients
                .Group(customerGroup)
                .SendAsync(RealtimeConstants.Events.ChatRoomUpdated, room, cancellationToken);
            return;
        }

        await _chatHubContext.Clients
            .Groups(customerGroup, hostGroup)
            .SendAsync(RealtimeConstants.Events.ChatRoomUpdated, room, cancellationToken);
    }

    public Task PublishChatMessagesReadAsync(Guid roomId, Guid readerUserId, DateTime readAt, int unreadCount, CancellationToken cancellationToken = default)
    {
        var payload = new
        {
            roomId,
            readerUserId,
            readAt,
            unreadCount
        };

        return _chatHubContext.Clients
            .Group(RealtimeConstants.Groups.ChatRoom(roomId))
            .SendAsync(RealtimeConstants.Events.ChatMessagesRead, payload, cancellationToken);
    }

    public Task PublishNotificationCreatedAsync(Guid userId, NotificationDto notification, CancellationToken cancellationToken = default)
    {
        return _notificationHubContext.Clients
            .Group(RealtimeConstants.Groups.User(userId))
            .SendAsync(RealtimeConstants.Events.NotificationCreated, notification, cancellationToken);
    }

    public Task PublishNotificationUnreadCountChangedAsync(Guid userId, int unreadCount, CancellationToken cancellationToken = default)
    {
        return _notificationHubContext.Clients
            .Group(RealtimeConstants.Groups.User(userId))
            .SendAsync(RealtimeConstants.Events.NotificationUnreadCountChanged, new { unreadCount }, cancellationToken);
    }

    public Task PublishNotificationReadAsync(Guid userId, Guid notificationId, DateTime readAt, int unreadCount, CancellationToken cancellationToken = default)
    {
        var payload = new
        {
            notificationId,
            readAt,
            unreadCount
        };

        return _notificationHubContext.Clients
            .Group(RealtimeConstants.Groups.User(userId))
            .SendAsync(RealtimeConstants.Events.NotificationRead, payload, cancellationToken);
    }

    public Task PublishNotificationReadAllAsync(Guid userId, DateTime readAt, int unreadCount, CancellationToken cancellationToken = default)
    {
        var payload = new
        {
            readAt,
            unreadCount
        };

        return _notificationHubContext.Clients
            .Group(RealtimeConstants.Groups.User(userId))
            .SendAsync(RealtimeConstants.Events.NotificationReadAll, payload, cancellationToken);
    }
}
