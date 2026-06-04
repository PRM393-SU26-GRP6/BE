using CourtManager.Application.DTOs;
using CourtManager.Application.Features.Chats;
using CourtManager.Application.Features.Chats.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CourtManager.APIs.Controllers;

/// <summary>
/// API endpoint for managing chat and messaging.
/// Provides endpoints for real-time communication between users.
/// </summary>
[ApiController]
[Route("api/v1/chats")]
[Authorize]
public class ChatsController : BaseApiController
{
    private readonly IMediator _mediator;
    private readonly ILogger<ChatsController> _logger;

    public ChatsController(IMediator mediator, ILogger<ChatsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Gets all chat rooms for the current user.
    /// </summary>
    /// <param name="pageNumber">Page number (default 1)</param>
    /// <param name="pageSize">Page size (default 10)</param>
    /// <returns>Paginated list of chat rooms</returns>
    [HttpGet("rooms")]
    [ProducesResponseType(typeof(IEnumerable<ChatRoomDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ChatRoomDto>>> GetChatRooms([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, CancellationToken cancellationToken = default)
    {
        var userId = CurrentUserId;
        _logger.LogInformation("Fetching chat rooms for user {UserId}", userId);
        var result = await _mediator.Send(new GetChatRoomsQuery(CurrentUserId, pageNumber, pageSize), cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Gets or creates a chat room with another user.
    /// </summary>
    /// <param name="otherUserId">The other user's ID</param>
    /// <returns>Chat room details</returns>
    [NonAction]
    [ProducesResponseType(typeof(ChatRoomDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ChatRoomDto>> GetOrCreateChatRoom(Guid otherUserId, CancellationToken cancellationToken = default)
    {
        var userId = CurrentUserId;
        _logger.LogInformation("Getting or creating chat room between users {UserId} and {OtherUserId}", userId, otherUserId);
        var result = await _mediator.Send(new GetOrCreateChatRoomQuery(CurrentUserId, otherUserId), cancellationToken);
        return Ok(result);
    }

    [NonAction]
    [ProducesResponseType(typeof(ChatRoomDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<ChatRoomDto>> GetOrCreateVenueChatRoom(Guid venueId, CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new GetOrCreateVenueChatRoomQuery(CurrentUserId, venueId), cancellationToken);
        return Ok(result);
    }

    [HttpPost("rooms")]
    [ProducesResponseType(typeof(ChatRoomDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<ChatRoomDto>> CreateOrGetRoom([FromBody] CreateChatRoomRequestDto request, CancellationToken cancellationToken = default)
    {
        ChatRoomDto result;
        if (request.VenueId.HasValue)
        {
            result = await _mediator.Send(new GetOrCreateVenueChatRoomQuery(CurrentUserId, request.VenueId.Value), cancellationToken);
        }
        else
        {
            var otherUserId = request.CustomerId == CurrentUserId ? request.OwnerId : request.CustomerId;
            if (otherUserId == Guid.Empty)
            {
                otherUserId = request.OwnerId;
            }

            result = await _mediator.Send(new GetOrCreateChatRoomQuery(CurrentUserId, otherUserId), cancellationToken);
        }

        return Ok(result);
    }

    /// <summary>
    /// Gets messages from a specific chat room.
    /// </summary>
    /// <param name="roomId">The chat room ID</param>
    /// <param name="pageNumber">Page number (default 1)</param>
    /// <param name="pageSize">Page size (default 20)</param>
    /// <returns>Paginated list of messages</returns>
    [HttpGet("rooms/{roomId}/messages")]
    [ProducesResponseType(typeof(IEnumerable<MessageDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessages(Guid roomId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Fetching messages for room {RoomId}", roomId);
        var result = await _mediator.Send(new GetMessagesQuery(CurrentUserId, roomId, pageNumber, pageSize), cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Sends a message in a chat room.
    /// </summary>
    /// <param name="roomId">The chat room ID</param>
    /// <param name="message">The message data</param>
    /// <returns>Created message</returns>
    [HttpPost("rooms/{roomId}/messages")]
    [ProducesResponseType(typeof(MessageDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<MessageDto>> SendMessage(Guid roomId, [FromBody] MessageDto message, CancellationToken cancellationToken = default)
    {
        var userId = CurrentUserId;
        _logger.LogInformation("Sending message to room {RoomId} from user {UserId}", roomId, userId);
        var result = await _mediator.Send(new SendMessageCommand(CurrentUserId, roomId, message.MessageText), cancellationToken);
        return CreatedAtAction(nameof(GetMessages), new { roomId }, result);
    }

    [NonAction]
    [ProducesResponseType(typeof(MessageDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<MessageDto>> SendMessageByBody([FromBody] MessageDto message, CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new SendMessageCommand(CurrentUserId, message.RoomId, message.MessageText), cancellationToken);
        return CreatedAtAction(nameof(GetMessages), new { roomId = message.RoomId }, result);
    }

    /// <summary>
    /// Deletes a message from a chat room (soft delete).
    /// </summary>
    /// <param name="roomId">The chat room ID</param>
    /// <param name="messageId">The message ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success status</returns>
    [NonAction]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> DeleteMessage(Guid roomId, Guid messageId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Deleting message {MessageId} from room {RoomId}", messageId, roomId);
        var command = new DeleteMessageCommand(messageId);
        var result = await _mediator.Send(command, cancellationToken);
        _logger.LogInformation("Message {MessageId} deleted successfully (soft delete)", messageId);
        return Ok(new { success = result, message = "Message deleted successfully" });
    }

    /// <summary>
    /// Closes/archives a chat room (soft delete).
    /// </summary>
    /// <param name="roomId">The chat room ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success status</returns>
    [NonAction]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CloseChatRoom(Guid roomId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Closing chat room {RoomId}", roomId);
        var command = new DeleteChatRoomCommand(roomId);
        var result = await _mediator.Send(command, cancellationToken);
        _logger.LogInformation("Chat room {RoomId} closed successfully (soft delete)", roomId);
        return Ok(new { success = result, message = "Chat room closed successfully" });
    }

    [HttpPut("rooms/{roomId:guid}/read")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> MarkRoomAsRead(Guid roomId, CancellationToken cancellationToken = default)
    {
        await _mediator.Send(new GetMessagesQuery(CurrentUserId, roomId, 1, 1), cancellationToken);
        return Ok(new { unreadCount = 0 });
    }


}

public class CreateChatRoomRequestDto
{
    public Guid CustomerId { get; set; }
    public Guid OwnerId { get; set; }
    public Guid? VenueId { get; set; }
    public Guid? BookingId { get; set; }
}
