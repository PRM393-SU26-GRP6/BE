using CourtManager.Application.DTOs;
using CourtManager.Application.Features.Reviews.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CourtManager.APIs.Controllers;

/// <summary>
/// API endpoint for managing reviews and ratings.
/// Provides CRUD operations for field reviews and ratings.
/// </summary>
[ApiController]
[Route("api/v1/reviews")]
[Authorize]
public class ReviewsController : BaseApiController
{
    private readonly IMediator _mediator;
    private readonly ILogger<ReviewsController> _logger;

    public ReviewsController(IMediator mediator, ILogger<ReviewsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Gets all reviews for a specific venue.
    /// </summary>
    /// <param name="id">The venue ID</param>
    /// <param name="page">Page number (default 1)</param>
    /// <param name="pageSize">Page size (default 10)</param>
    /// <returns>Paginated list of reviews and average rating</returns>
    [HttpGet("venue/{id}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(VenueReviewsResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetVenueReviews(Guid id, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var query = new CourtManager.Application.Features.Reviews.Queries.GetVenueReviewsQuery(id, page, pageSize);
        var result = await _mediator.Send(query);

        return Ok(new
        {
            success = true,
            message = "OK",
            data = result,
            errors = Array.Empty<string>()
        });
    }

    /// <summary>
    /// Gets a specific review by ID.
    /// </summary>
    /// <param name="id">The review ID</param>
    /// <returns>Review details</returns>
    [HttpGet("{id}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ReviewDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetReviewById(Guid id)
    {
        _logger.LogInformation("Fetching review {ReviewId}", id);
        return Ok(new { message = "Get review by ID endpoint - implementation pending" });
    }

    /// <summary>
    /// Gets average rating for a field.
    /// </summary>
    /// <param name="fieldId">The field ID</param>
    /// <returns>Average rating</returns>
    [HttpGet("field/{fieldId}/average-rating")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetAverageRating(Guid fieldId)
    {
        _logger.LogInformation("Fetching average rating for field {FieldId}", fieldId);
        return Ok(new { message = "Get average rating endpoint - implementation pending" });
    }

    /// <summary>
    /// Gets all reviews by the current user.
    /// </summary>
    /// <returns>List of user's reviews</returns>
    [HttpGet("my-reviews")]
    [ProducesResponseType(typeof(IEnumerable<ReviewDto>), StatusCodes.Status200OK)]
    public IActionResult GetMyReviews()
    {
        var userId = CurrentUserId;
        _logger.LogInformation("Fetching reviews for current user {UserId}", userId);
        return Ok(new { message = "Get my reviews endpoint - implementation pending" });
    }

    /// <summary>
    /// Creates a new review for a venue after a completed booking.
    /// </summary>
    /// <param name="request">The review creation data</param>
    /// <returns>Created review</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ReviewDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CreateReview([FromBody] CreateReviewRequestDto request)
    {
        if (request.Rating < 1 || request.Rating > 5)
        {
            return BadRequest(new { success = false, message = "Rating must be between 1 and 5." });
        }

        try
        {
            var command = new CourtManager.Application.Features.Reviews.Commands.CreateReviewCommand(
                request.VenueId, request.BookingId, request.Rating, request.Comment, CurrentUserId);
            
            var result = await _mediator.Send(command);

            return CreatedAtAction(nameof(GetReviewById), new { id = result.ReviewId }, new
            {
                success = true,
                message = "Review created successfully",
                data = result,
                errors = Array.Empty<string>()
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = "Failed to create review", errors = new[] { ex.Message } });
        }
    }

    /// <summary>
    /// Updates an existing review (Owner only).
    /// </summary>
    /// <param name="id">The review ID</param>
    /// <param name="request">The updated review data</param>
    /// <returns>Updated review</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ReviewDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UpdateReview(Guid id, [FromBody] UpdateReviewRequestDto request)
    {
        _logger.LogInformation("Updating review {ReviewId}", id);
        
        if (request.Rating < 1 || request.Rating > 5)
        {
            return BadRequest(new { success = false, message = "Rating must be between 1 and 5." });
        }

        try
        {
            var command = new CourtManager.Application.Features.Reviews.Commands.UpdateReviewCommand(
                id, request.Rating, request.Comment, CurrentUserId);

            var result = await _mediator.Send(command);

            return Ok(new
            {
                success = true,
                message = "Review updated successfully",
                data = result,
                errors = Array.Empty<string>()
            });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { success = false, message = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid();
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = "Failed to update review", errors = new[] { ex.Message } });
        }
    }

    /// <summary>
    /// Deletes a review (soft delete - Owner/Admin only).
    /// </summary>
    /// <param name="id">The review ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success status</returns>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin,Owner")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> DeleteReview(Guid id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Deleting review {ReviewId}", id);
        var command = new DeleteReviewCommand(id);
        var result = await _mediator.Send(command, cancellationToken);
        _logger.LogInformation("Review {ReviewId} deleted successfully (soft delete)", id);
        return Ok(new { success = result, message = "Review deleted successfully" });
    }
}
