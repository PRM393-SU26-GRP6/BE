using System.Security.Claims;
using CourtManager.APIs.Services.Realtime;
using CourtManager.Application.Features.Notifications;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace CourtManager.APIs.Hubs;

[Authorize]
public class NotificationHub : Hub
{
    private readonly IMediator _mediator;
    private readonly ILogger<NotificationHub> _logger;

    public NotificationHub(IMediator mediator, ILogger<NotificationHub> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public override async Task OnConnectedAsync()
    {
        var userId = GetCurrentUserId();
        await Groups.AddToGroupAsync(Context.ConnectionId, RealtimeConstants.Groups.User(userId), Context.ConnectionAborted);
        await base.OnConnectedAsync();
    }

    public async Task GetUnreadCount()
    {
        try
        {
            var userId = GetCurrentUserId();
            var unreadCount = await _mediator.Send(new GetUnreadNotificationCountQuery(userId), Context.ConnectionAborted);
            await Clients.Caller.SendAsync(RealtimeConstants.Events.NotificationUnreadCountChanged, new { unreadCount }, Context.ConnectionAborted);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to get notification unread count");
            await SendNotificationErrorAsync(ex.Message);
        }
    }

    public async Task MarkNotificationAsRead(Guid notificationId)
    {
        try
        {
            var userId = GetCurrentUserId();
            await _mediator.Send(new MarkNotificationAsReadCommand(notificationId, userId), Context.ConnectionAborted);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to mark notification {NotificationId} as read", notificationId);
            await SendNotificationErrorAsync(ex.Message);
        }
    }

    public async Task MarkAllNotificationsAsRead()
    {
        try
        {
            var userId = GetCurrentUserId();
            await _mediator.Send(new MarkAllNotificationsAsReadCommand(userId), Context.ConnectionAborted);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to mark all notifications as read");
            await SendNotificationErrorAsync(ex.Message);
        }
    }

    private Guid GetCurrentUserId()
    {
        var raw = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(raw, out var userId))
        {
            throw new HubException("Invalid or missing user identity claim.");
        }

        return userId;
    }

    private Task SendNotificationErrorAsync(string message)
    {
        return Clients.Caller.SendAsync(RealtimeConstants.Events.NotificationError, new { message }, Context.ConnectionAborted);
    }
}
