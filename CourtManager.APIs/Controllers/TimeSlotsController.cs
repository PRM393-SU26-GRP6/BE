using CourtManager.Application.DTOs;
using CourtManager.Application.Features.TimeSlots.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CourtManager.APIs.Controllers;

/// <summary>
/// API endpoint for managing time slots.
/// Provides CRUD operations and query endpoints for time slots.
/// </summary>
[ApiController]
[Route("api/v1/slots")]
[Authorize]
public class TimeSlotsController : BaseApiController
{
    private readonly IMediator _mediator;
    private readonly ILogger<TimeSlotsController> _logger;

    public TimeSlotsController(IMediator mediator, ILogger<TimeSlotsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Gets all time slots for a specific field.
    /// </summary>
    /// <param name="fieldId">The field ID</param>
    /// <returns>List of time slots</returns>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(IEnumerable<TimeSlotDto>), StatusCodes.Status200OK)]
    public IActionResult GetSlotsByField([FromQuery] Guid fieldId)
    {
        _logger.LogInformation("Fetching time slots for field {FieldId}", fieldId);
        return Ok(new { message = "Get slots by field endpoint - implementation pending" });
    }

    /// <summary>
    /// Gets a specific time slot by ID.
    /// </summary>
    /// <param name="id">The time slot ID</param>
    /// <returns>Time slot details</returns>
    [HttpGet("{id}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(TimeSlotDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetSlotById(Guid id)
    {
        _logger.LogInformation("Fetching time slot {SlotId}", id);
        return Ok(new { message = "Get slot by ID endpoint - implementation pending" });
    }

    /// <summary>
    /// Gets available time slots for a field on a specific date.
    /// </summary>
    /// <param name="fieldId">The field ID</param>
    /// <param name="date">The date</param>
    /// <returns>List of available time slots</returns>
    [HttpGet("available")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(IEnumerable<TimeSlotDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAvailableSlots([FromQuery] Guid fieldId, [FromQuery] DateTime date)
    {
        var query = new CourtManager.Application.Features.TimeSlots.Queries.GetAvailableSlotsQuery(fieldId, date);
        var result = await _mediator.Send(query);

        return Ok(new
        {
            success = true,
            message = "OK",
            data = result,
            errors = Array.Empty<string>()
        });
    }

    [HttpPost("{id}/lock")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> LockSlot(Guid id)
    {
        try
        {
            var userId = CurrentUserId;
            var command = new CourtManager.Application.Features.TimeSlots.Commands.LockSlotCommand(id, userId);
            var result = await _mediator.Send(command);

            return Ok(new
            {
                success = true,
                message = "Slot locked successfully for 15 minutes.",
                data = new { },
                errors = Array.Empty<string>()
            });
        }
        catch (CourtManager.Application.Exceptions.NotFoundException ex)
        {
            return NotFound(new { success = false, message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = "Failed to lock slot", errors = new[] { ex.Message } });
        }
    }

    [HttpPost("{id}/unlock")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UnlockSlot(Guid id)
    {
        try
        {
            var userId = CurrentUserId;
            var command = new CourtManager.Application.Features.TimeSlots.Commands.UnlockSlotCommand(id, userId);
            var result = await _mediator.Send(command);

            return Ok(new
            {
                success = true,
                message = "Slot unlocked successfully.",
                data = new { },
                errors = Array.Empty<string>()
            });
        }
        catch (CourtManager.Application.Exceptions.NotFoundException ex)
        {
            return NotFound(new { success = false, message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = "Failed to unlock slot", errors = new[] { ex.Message } });
        }
    }

    /// <summary>
    /// Creates a new time slot (Manager/Admin only).
    /// </summary>
    /// <param name="slot">The time slot creation data</param>
    /// <returns>Created time slot</returns>
    [HttpPost]
    [Authorize(Roles = "Owner,Admin")]
    [ProducesResponseType(typeof(TimeSlotDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult CreateSlot([FromBody] TimeSlotDto slot)
    {
        _logger.LogInformation("Creating new time slot for field {FieldId}", slot.FieldId);
        return Ok(new { message = "Create slot endpoint - implementation pending" });
    }

    /// <summary>
    /// Updates a time slot (Manager/Admin only).
    /// </summary>
    /// <param name="id">The time slot ID</param>
    /// <param name="slot">The updated time slot data</param>
    /// <returns>Updated time slot</returns>
    [HttpPut("{id}")]
    [Authorize(Roles = "Owner,Admin")]
    [ProducesResponseType(typeof(TimeSlotDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult UpdateSlot(Guid id, [FromBody] TimeSlotDto slot)
    {
        _logger.LogInformation("Updating time slot {SlotId}", id);
        return Ok(new { message = "Update slot endpoint - implementation pending" });
    }

    /// <summary>
    /// Deletes a time slot (soft delete - Manager/Admin only).
    /// </summary>
    /// <param name="id">The time slot ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success status</returns>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Owner,Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> DeleteSlot(Guid id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Deleting time slot {SlotId}", id);
        var command = new DeleteTimeSlotCommand(id);
        var result = await _mediator.Send(command, cancellationToken);
        _logger.LogInformation("Time slot {SlotId} deleted successfully (soft delete)", id);
        return Ok(new { success = result, message = "Time slot deleted successfully" });
    }
}
