using CourtManager.Application.DTOs;

namespace CourtManager.APIs.Services.Realtime;

public interface IRealtimeEventPublisher
{
    Task PublishChatMessageCreatedAsync(MessageDto message, CancellationToken cancellationToken = default);
    Task PublishChatRoomUpdatedAsync(ChatRoomDto room, CancellationToken cancellationToken = default);
    Task PublishChatMessagesReadAsync(Guid roomId, Guid readerUserId, DateTime readAt, int unreadCount, CancellationToken cancellationToken = default);
    Task PublishNotificationCreatedAsync(Guid userId, NotificationDto notification, CancellationToken cancellationToken = default);
    Task PublishNotificationUnreadCountChangedAsync(Guid userId, int unreadCount, CancellationToken cancellationToken = default);
    Task PublishNotificationReadAsync(Guid userId, Guid notificationId, DateTime readAt, int unreadCount, CancellationToken cancellationToken = default);
    Task PublishNotificationReadAllAsync(Guid userId, DateTime readAt, int unreadCount, CancellationToken cancellationToken = default);
}
