using CourtManager.Application.DTOs;
using CourtManager.Application.Features.Notifications;
using CourtManager.Application.Features.Notifications.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CourtManager.APIs.Controllers;

/// <summary>
/// API endpoint for managing notifications.
/// Provides endpoints for retrieving and managing user notifications.
/// </summary>
[ApiController]
[Route("api/v1/notifications")]
[Authorize]
public class NotificationsController : BaseApiController
{
    private readonly IMediator _mediator;
    private readonly ILogger<NotificationsController> _logger;

    public NotificationsController(IMediator mediator, ILogger<NotificationsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Gets all notifications for the current user.
    /// </summary>
    /// <param name="unreadOnly">Filter to show only unread notifications</param>
    /// <param name="pageNumber">Page number (default 1)</param>
    /// <param name="pageSize">Page size (default 10)</param>
    /// <returns>Paginated list of notifications</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<NotificationDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<NotificationDto>>> GetNotifications([FromQuery] bool unreadOnly = false, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, CancellationToken cancellationToken = default)
    {
        var userId = CurrentUserId;
        _logger.LogInformation("Fetching notifications for user {UserId}", userId);
        var result = await _mediator.Send(new GetNotificationsQuery(GetCurrentUserId(), unreadOnly, pageNumber, pageSize), cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Gets a specific notification by ID.
    /// </summary>
    /// <param name="id">The notification ID</param>
    /// <returns>Notification details</returns>
    [NonAction]
    [ProducesResponseType(typeof(NotificationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<NotificationDto>> GetNotificationById(Guid id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Fetching notification {NotificationId}", id);
        var result = await _mediator.Send(new GetNotificationByIdQuery(id, GetCurrentUserId()), cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Gets count of unread notifications for the current user.
    /// </summary>
    /// <returns>Unread notification count</returns>
    [HttpGet("unread-count")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public async Task<ActionResult<object>> GetUnreadCount(CancellationToken cancellationToken = default)
    {
        var userId = CurrentUserId;
        _logger.LogInformation("Fetching unread notification count for user {UserId}", userId);
        var result = await _mediator.Send(new GetUnreadNotificationCountQuery(GetCurrentUserId()), cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Marks a notification as read.
    /// </summary>
    /// <param name="id">The notification ID</param>
    /// <returns>Success status</returns>
    [HttpPut("{id:guid}/read")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> MarkAsRead(Guid id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Marking notification {NotificationId} as read", id);
        var result = await _mediator.Send(new MarkNotificationAsReadCommand(id, GetCurrentUserId()), cancellationToken);
        return Ok(new { success = result });
    }

    /// <summary>
    /// Marks all notifications as read for the current user.
    /// </summary>
    /// <returns>Success status</returns>
    [HttpPut("read-all")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> MarkAllAsRead(CancellationToken cancellationToken = default)
    {
        var userId = CurrentUserId;
        _logger.LogInformation("Marking all notifications as read for user {UserId}", userId);
        var result = await _mediator.Send(new MarkAllNotificationsAsReadCommand(GetCurrentUserId()), cancellationToken);
        return Ok(new { success = result });
    }

    /// <summary>
    /// Deletes a notification (soft delete).
    /// </summary>
    /// <param name="id">The notification ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success status</returns>
    [NonAction]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> DeleteNotification(Guid id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Deleting notification {NotificationId}", id);
        var command = new DeleteNotificationCommand(id);
        var result = await _mediator.Send(command, cancellationToken);
        _logger.LogInformation("Notification {NotificationId} deleted successfully (soft delete)", id);
        return Ok(new { success = result, message = "Notification deleted successfully" });
    }

    private Guid GetCurrentUserId()
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(userIdString, out var userId) ? userId : Guid.Empty;
    }
}
