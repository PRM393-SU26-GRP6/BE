using System.Security.Claims;
using CourtManager.Application.DTOs;
using CourtManager.Application.Exceptions;
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
        var userId = CurrentUserId();
        await Groups.AddToGroupAsync(Context.ConnectionId, UserGroup(userId));
        await base.OnConnectedAsync();
    }

    public async Task JoinRoom(Guid roomId)
    {
        try
        {
            await GetRoomForCurrentUser(roomId);
            await Groups.AddToGroupAsync(Context.ConnectionId, RoomGroup(roomId));
        }
        catch (Exception ex)
        {
            await SendHubError(ex);
        }
    }

    public async Task LeaveRoom(Guid roomId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, RoomGroup(roomId));
    }

    public async Task SendMessage(Guid roomId, string messageText, string? clientMessageId = null)
    {
        try
        {
            var senderId = CurrentUserId();
            var room = await GetRoomForCurrentUser(roomId);
            var message = await _mediator.Send(new SendMessageCommand(senderId, roomId, messageText));

            var messagePayload = new ChatMessageCreatedPayload(
                message.MessageId,
                message.RoomId,
                message.SenderId,
                message.SenderName,
                message.MessageText,
                message.SentAt,
                clientMessageId);

            await Clients.Group(RoomGroup(roomId)).SendAsync("chat.messageCreated", messagePayload);

            var recipientId = room.CustomerId == senderId ? room.HostId : room.CustomerId;
            await SendRoomUpdated(senderId, roomId, message);
            await SendRoomUpdated(recipientId, roomId, message);
        }
        catch (Exception ex)
        {
            await SendHubError(ex);
        }
    }

    public async Task Typing(Guid roomId)
    {
        try
        {
            var userId = CurrentUserId();
            await GetRoomForCurrentUser(roomId);

            await Clients.OthersInGroup(RoomGroup(roomId)).SendAsync(
                "chat.typing",
                new ChatTypingPayload(roomId, userId, true));
        }
        catch (Exception ex)
        {
            await SendHubError(ex);
        }
    }

    public async Task MarkRoomAsRead(Guid roomId)
    {
        try
        {
            var readerId = CurrentUserId();
            await _mediator.Send(new MarkRoomAsReadCommand(readerId, roomId));

            var readAt = DateTime.UtcNow;
            await Clients.Group(RoomGroup(roomId)).SendAsync(
                "chat.messageRead",
                new ChatMessageReadPayload(roomId, readerId, readAt));

            var room = await _mediator.Send(new GetChatRoomForUserQuery(readerId, roomId));
            await Clients.Group(UserGroup(readerId)).SendAsync(
                "chat.roomUpdated",
                new ChatRoomUpdatedPayload(
                    room.RoomId,
                    room.LastMessagePreview,
                    room.LastMessageTime,
                    room.UnreadCount));
        }
        catch (Exception ex)
        {
            await SendHubError(ex);
        }
    }

    private async Task<ChatRoomDto> GetRoomForCurrentUser(Guid roomId)
    {
        return await _mediator.Send(new GetChatRoomForUserQuery(CurrentUserId(), roomId));
    }

    private async Task SendRoomUpdated(Guid userId, Guid roomId, MessageDto message)
    {
        var room = await _mediator.Send(new GetChatRoomForUserQuery(userId, roomId));

        await Clients.Group(UserGroup(userId)).SendAsync(
            "chat.roomUpdated",
            new ChatRoomUpdatedPayload(
                room.RoomId,
                message.MessageText,
                message.SentAt,
                room.UnreadCount));
    }

    private Guid CurrentUserId()
    {
        var raw = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(raw, out var userId))
        {
            throw new UnauthorizedAccessException("Invalid or missing user identity claim.");
        }

        return userId;
    }

    private async Task SendHubError(Exception exception)
    {
        _logger.LogWarning(exception, "ChatHub operation failed.");

        var (code, message) = exception switch
        {
            ValidationException => ("validation_error", exception.Message),
            NotFoundException => ("not_found", exception.Message),
            ForbiddenException => ("forbidden", exception.Message),
            UnauthorizedAccessException => ("unauthorized", exception.Message),
            _ => ("server_error", "Chat operation failed.")
        };

        await Clients.Caller.SendAsync("chat.error", new ChatErrorPayload(code, message));
    }

    private static string UserGroup(Guid userId) => $"user:{userId}";

    private static string RoomGroup(Guid roomId) => $"chatroom:{roomId}";

    private sealed record ChatMessageCreatedPayload(
        Guid MessageId,
        Guid RoomId,
        Guid SenderId,
        string? SenderName,
        string MessageText,
        DateTime SentAt,
        string? ClientMessageId);

    private sealed record ChatRoomUpdatedPayload(
        Guid RoomId,
        string? LastMessagePreview,
        DateTime? LastMessageTime,
        int UnreadCount);

    private sealed record ChatTypingPayload(
        Guid RoomId,
        Guid UserId,
        bool IsTyping);

    private sealed record ChatMessageReadPayload(
        Guid RoomId,
        Guid ReaderId,
        DateTime ReadAt);

    private sealed record ChatErrorPayload(
        string Code,
        string Message);
}
