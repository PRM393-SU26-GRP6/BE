using System.Security.Claims;
using CourtManager.APIs.Services.Realtime;
using CourtManager.Application.Features.Chats;
using CourtManager.Application.Features.Notifications;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace CourtManager.APIs.Hubs;

[Authorize]
public class ChatHub : Hub
{
    private readonly IMediator _mediator;
    private readonly IRealtimeEventPublisher _publisher;
    private readonly ILogger<ChatHub> _logger;

    public ChatHub(IMediator mediator, IRealtimeEventPublisher publisher, ILogger<ChatHub> logger)
    {
        _mediator = mediator;
        _publisher = publisher;
        _logger = logger;
    }

    public override async Task OnConnectedAsync()
    {
        var userId = GetCurrentUserId();
        await Groups.AddToGroupAsync(Context.ConnectionId, RealtimeConstants.Groups.User(userId), Context.ConnectionAborted);
        await base.OnConnectedAsync();
    }

    public async Task JoinRoom(Guid roomId)
    {
        try
        {
            var userId = GetCurrentUserId();
            var room = await _mediator.Send(new GetChatRoomByIdQuery(userId, roomId), Context.ConnectionAborted);

            await Groups.AddToGroupAsync(Context.ConnectionId, RealtimeConstants.Groups.ChatRoom(roomId), Context.ConnectionAborted);
            await Clients.Caller.SendAsync(RealtimeConstants.Events.ChatRoomJoined, room, Context.ConnectionAborted);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to join chat room {RoomId}", roomId);
            await SendChatErrorAsync(ex.Message);
        }
    }

    public async Task LeaveRoom(Guid roomId)
    {
        try
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, RealtimeConstants.Groups.ChatRoom(roomId), Context.ConnectionAborted);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to leave chat room {RoomId}", roomId);
            await SendChatErrorAsync(ex.Message);
        }
    }

    public async Task StartTyping(Guid roomId)
    {
        await PublishTypingAsync(roomId, RealtimeConstants.Events.ChatTypingStarted);
    }

    public async Task StopTyping(Guid roomId)
    {
        await PublishTypingAsync(roomId, RealtimeConstants.Events.ChatTypingStopped);
    }

    public async Task SendMessage(Guid roomId, string messageText)
    {
        try
        {
            var userId = GetCurrentUserId();
            var message = await _mediator.Send(new SendMessageCommand(userId, roomId, messageText), Context.ConnectionAborted);
            var room = await _mediator.Send(new GetChatRoomByIdQuery(userId, roomId), Context.ConnectionAborted);
            var recipientId = room.CustomerId == userId ? room.HostId : room.CustomerId;
            var unreadCount = await _mediator.Send(new GetUnreadNotificationCountValueQuery(recipientId), Context.ConnectionAborted);

            await _publisher.PublishChatMessageCreatedAsync(message, Context.ConnectionAborted);
            await _publisher.PublishChatRoomUpdatedAsync(room, Context.ConnectionAborted);
            await _publisher.PublishNotificationUnreadCountChangedAsync(recipientId, unreadCount, Context.ConnectionAborted);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to send chat message to room {RoomId}", roomId);
            await SendChatErrorAsync(ex.Message);
        }
    }

    public async Task MarkRoomAsRead(Guid roomId)
    {
        try
        {
            var userId = GetCurrentUserId();
            var readAt = DateTime.UtcNow;
            var unreadCount = await _mediator.Send(new MarkRoomAsReadCommand(userId, roomId), Context.ConnectionAborted);
            await _publisher.PublishChatMessagesReadAsync(roomId, userId, readAt, unreadCount, Context.ConnectionAborted);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to mark chat room {RoomId} as read", roomId);
            await SendChatErrorAsync(ex.Message);
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

    private Task SendChatErrorAsync(string message)
    {
        return Clients.Caller.SendAsync(RealtimeConstants.Events.ChatError, new { message }, Context.ConnectionAborted);
    }

    private async Task PublishTypingAsync(Guid roomId, string eventName)
    {
        try
        {
            var userId = GetCurrentUserId();
            await _mediator.Send(new GetChatRoomByIdQuery(userId, roomId), Context.ConnectionAborted);

            await Clients.OthersInGroup(RealtimeConstants.Groups.ChatRoom(roomId))
                .SendAsync(eventName, new
                {
                    roomId,
                    userId,
                    connectionId = Context.ConnectionId,
                    at = DateTime.UtcNow
                }, Context.ConnectionAborted);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to publish typing event {EventName} for room {RoomId}", eventName, roomId);
            await SendChatErrorAsync(ex.Message);
        }
    }
}
