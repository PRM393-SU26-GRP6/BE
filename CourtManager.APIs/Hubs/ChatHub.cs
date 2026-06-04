using System.Security.Claims;
using CourtManager.APIs.Services.Realtime;
using CourtManager.Application.Features.Chats;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace CourtManager.APIs.Hubs;

[Authorize]
public class ChatHub : Hub
{
    private readonly IMediator _mediator;
    private readonly ILogger<ChatHub> _logger;

    public ChatHub(IMediator mediator, ILogger<ChatHub> logger)
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

    public async Task SendMessage(Guid roomId, string messageText)
    {
        try
        {
            var userId = GetCurrentUserId();
            await _mediator.Send(new SendMessageCommand(userId, roomId, messageText), Context.ConnectionAborted);
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
            await _mediator.Send(new MarkRoomAsReadCommand(userId, roomId), Context.ConnectionAborted);
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
}
