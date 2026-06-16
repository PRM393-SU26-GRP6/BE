using CourtManager.Application.DTOs;
using CourtManager.Application.Exceptions;
using CourtManager.Domain.Entities;
using CourtManager.Application.Interfaces;
using MediatR;

namespace CourtManager.Application.Features.Notifications;

public record GetNotificationsQuery(Guid UserId, bool UnreadOnly, int PageNumber, int PageSize) : IRequest<IEnumerable<NotificationDto>>;
public record GetNotificationByIdQuery(Guid NotificationId, Guid UserId) : IRequest<NotificationDto>;
public record GetUnreadNotificationCountQuery(Guid UserId) : IRequest<int>;
public record MarkNotificationAsReadCommand(Guid NotificationId, Guid UserId) : IRequest<bool>;
public record MarkAllNotificationsAsReadCommand(Guid UserId) : IRequest<bool>;

public class GetNotificationsQueryHandler : IRequestHandler<GetNotificationsQuery, IEnumerable<NotificationDto>>
{
    private readonly INotificationRepository _notificationRepository;

    public GetNotificationsQueryHandler(INotificationRepository notificationRepository)
    {
        _notificationRepository = notificationRepository;
    }

    public async Task<IEnumerable<NotificationDto>> Handle(GetNotificationsQuery request, CancellationToken cancellationToken)
    {
        var notifications = await _notificationRepository.GetNotificationsByUserIdPaginatedAsync(
            request.UserId,
            request.PageNumber,
            request.PageSize,
            request.UnreadOnly,
            cancellationToken);

        return notifications.Select(n => ToDto(n, request.UserId));
    }

    internal static NotificationDto ToDto(Notification notification, Guid userId)
    {
        var recipient = notification.Recipients.FirstOrDefault(r => r.UserId == userId);
        return new NotificationDto
        {
            NotificationId = notification.NotificationId,
            UserId = userId,
            Title = notification.Title,
            Message = notification.Message,
            Type = notification.Type.ToString(),
            RefId = notification.RefId,
            IsRead = recipient?.ReadAt != null,
            ReadAt = recipient?.ReadAt,
            CreatedAt = notification.CreatedAt
        };
    }
}

public class GetNotificationByIdQueryHandler : IRequestHandler<GetNotificationByIdQuery, NotificationDto>
{
    private readonly INotificationRepository _notificationRepository;

    public GetNotificationByIdQueryHandler(INotificationRepository notificationRepository)
    {
        _notificationRepository = notificationRepository;
    }

    public async Task<NotificationDto> Handle(GetNotificationByIdQuery request, CancellationToken cancellationToken)
    {
        var notifications = await _notificationRepository.GetNotificationsByUserIdAsync(request.UserId, false, cancellationToken);
        var notification = notifications.FirstOrDefault(n => n.NotificationId == request.NotificationId);
        if (notification == null)
            throw new NotFoundException(nameof(Notification), request.NotificationId);

        return GetNotificationsQueryHandler.ToDto(notification, request.UserId);
    }
}

public class GetUnreadNotificationCountQueryHandler : IRequestHandler<GetUnreadNotificationCountQuery, int>
{
    private readonly INotificationRepository _notificationRepository;

    public GetUnreadNotificationCountQueryHandler(INotificationRepository notificationRepository)
    {
        _notificationRepository = notificationRepository;
    }

    public async Task<int> Handle(GetUnreadNotificationCountQuery request, CancellationToken cancellationToken)
    {
        return await _notificationRepository.GetUnreadCountAsync(request.UserId, cancellationToken);
    }
}

public class MarkNotificationAsReadCommandHandler : IRequestHandler<MarkNotificationAsReadCommand, bool>
{
    private readonly INotificationRepository _notificationRepository;
    private readonly IRealtimeEventPublisher _realtimePublisher;

    public MarkNotificationAsReadCommandHandler(
        INotificationRepository notificationRepository,
        IRealtimeEventPublisher realtimePublisher)
    {
        _notificationRepository = notificationRepository;
        _realtimePublisher = realtimePublisher;
    }

    public async Task<bool> Handle(MarkNotificationAsReadCommand request, CancellationToken cancellationToken)
    {
        var readAt = DateTime.UtcNow;
        await _notificationRepository.MarkAsReadAsync(request.NotificationId, request.UserId, cancellationToken);
        await _notificationRepository.SaveChangesAsync(cancellationToken);
        var unreadCount = await _notificationRepository.GetUnreadCountAsync(request.UserId, cancellationToken);

        await _realtimePublisher.PublishNotificationReadAsync(request.UserId, request.NotificationId, readAt, unreadCount, cancellationToken);
        await _realtimePublisher.PublishNotificationUnreadCountChangedAsync(request.UserId, unreadCount, cancellationToken);

        return true;
    }
}

public class MarkAllNotificationsAsReadCommandHandler : IRequestHandler<MarkAllNotificationsAsReadCommand, bool>
{
    private readonly INotificationRepository _notificationRepository;
    private readonly IRealtimeEventPublisher _realtimePublisher;

    public MarkAllNotificationsAsReadCommandHandler(
        INotificationRepository notificationRepository,
        IRealtimeEventPublisher realtimePublisher)
    {
        _notificationRepository = notificationRepository;
        _realtimePublisher = realtimePublisher;
    }

    public async Task<bool> Handle(MarkAllNotificationsAsReadCommand request, CancellationToken cancellationToken)
    {
        var readAt = DateTime.UtcNow;
        await _notificationRepository.MarkAllAsReadAsync(request.UserId, cancellationToken);
        await _notificationRepository.SaveChangesAsync(cancellationToken);
        var unreadCount = await _notificationRepository.GetUnreadCountAsync(request.UserId, cancellationToken);

        await _realtimePublisher.PublishNotificationReadAllAsync(request.UserId, readAt, unreadCount, cancellationToken);
        await _realtimePublisher.PublishNotificationUnreadCountChangedAsync(request.UserId, unreadCount, cancellationToken);

        return true;
    }
}
