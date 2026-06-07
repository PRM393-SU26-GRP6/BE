using CourtManager.Application.Features.Bookings.Commands;
using CourtManager.Application.Features.Bookings.Queries;
using CourtManager.Application.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CourtManager.APIs.Controllers;

/// <summary>
/// API endpoint for managing bookings.
/// Provides CRUD operations and query endpoints for bookings.
/// </summary>
[ApiController]
[Route("api/v1/bookings")]
[Authorize]
public class BookingsController : BaseApiController
{
    private readonly IMediator _mediator;
    private readonly ILogger<BookingsController> _logger;

    public BookingsController(IMediator mediator, ILogger<BookingsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Creates a new booking.
    /// </summary>
    /// <param name="command">The booking creation command</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The created booking</returns>
    [HttpPost]
    [ProducesResponseType(typeof(BookingDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<BookingDto>> CreateBooking(
        [FromBody] CreateBookingCommand command,
        CancellationToken cancellationToken = default)
    {
        var currentUserId = CurrentUserId;

        // Force the command to use the logged-in user's ID
        // (Prevents IDOR: User creating booking for someone else)
        command.UserId = currentUserId;

        _logger.LogInformation(
            "Creating booking for User: {UserId} with {SlotCount} slots",
            command.UserId, command.SlotIds.Length);

        var result = await _mediator.Send(command, cancellationToken);
        _logger.LogInformation("Booking created successfully with ID: {BookingId}", result.Id);

        return CreatedAtAction(nameof(GetBookingById), new { id = result.Id }, new
        {
            success = true,
            message = "Booking created successfully",
            data = result,
            errors = Array.Empty<string>()
        });
    }

    /// <summary>
    /// Retrieves a booking by ID.
    /// </summary>
    /// <param name="id">The booking ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The booking details</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(BookingDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<BookingDto>> GetBookingById(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Fetching booking with ID: {BookingId}", id);

        var currentUserIdStr = CurrentUserId.ToString();
        var isOwner = User.IsInRole("Owner");
        var isAdmin = User.IsInRole("Admin");

        var query = new GetBookingByIdQuery(id, CurrentUserId, isOwner, isAdmin);
        var result = await _mediator.Send(query, cancellationToken);

        // Resource-based Authorization: Only Admin/Manager or the owner can view this booking
        var isAdminOrManager = isAdmin || isOwner;

        if (!isAdminOrManager && result.UserId.ToString() != currentUserIdStr)
        {
            _logger.LogWarning("User {UserId} attempted to access booking {BookingId} which they do not own.", currentUserIdStr, id);
            return Forbid();
        }

        return Ok(result);
    }

    /// <summary>
    /// Retrieves booking history for the current customer.
    /// </summary>
    /// <param name="status">Optional status filter</param>
    /// <param name="from">Optional start date</param>
    /// <param name="to">Optional end date</param>
    /// <param name="page">Page number</param>
    /// <param name="pageSize">Page size</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of bookings</returns>
    [HttpGet("history")]
    [ProducesResponseType(typeof(IEnumerable<BookingDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<BookingDto>>> GetBookingHistory(
        [FromQuery] string? status = null,
        [FromQuery] DateTime? from = null,
        [FromQuery] DateTime? to = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Fetching booking history for user: {UserId}", CurrentUserId);

        var query = new GetUserBookingsQuery(CurrentUserId, status, from, to, page, pageSize);
        var result = await _mediator.Send(query, cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// Checks if a booking already has a review.
    /// </summary>
    /// <param name="id">The booking ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The review details or null if no review exists</returns>
    [HttpGet("{id}/review")]
    [ProducesResponseType(typeof(CourtManager.Application.DTOs.ReviewDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetBookingReview(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Checking review for booking with ID: {BookingId}", id);

        var query = new CourtManager.Application.Features.Reviews.Queries.GetBookingReviewQuery(id);
        var result = await _mediator.Send(query, cancellationToken);

        return Ok(new
        {
            success = true,
            message = "OK",
            data = result,
            errors = Array.Empty<string>()
        });
    }



    /// <summary>
    /// Cancels an existing booking.
    /// Can cancel Pending or Confirmed bookings.
    /// </summary>
    /// <param name="id">The booking ID</param>
    /// <param name="cancellationReason">Optional reason for cancellation</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success status</returns>
    [HttpPut("{id}/cancel")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CancelBooking(
        Guid id,
        [FromQuery] string? cancellationReason = null,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Cancelling booking with ID: {BookingId}", id);

        var command = new CancelBookingCommand(id, CurrentUserId, false, cancellationReason);
        var result = await _mediator.Send(command, cancellationToken);

        _logger.LogInformation("Booking {BookingId} cancelled successfully", id);

        return Ok(new { success = result, message = "Booking cancelled successfully" });
    }

    /// <summary>
    /// Health check endpoint for API.
    /// </summary>
    [HttpGet("health")]
    [AllowAnonymous]
    public IActionResult Health()
    {
        return Ok(new { status = "API is running" });
    }
}
