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
    public async Task<IActionResult> GetSlotsByField([FromQuery] Guid fieldId)
    {
        _logger.LogInformation("Fetching time slots for field {FieldId}", fieldId);
        var result = await _mediator.Send(new CourtManager.Application.Features.TimeSlots.GetSlotsByFieldQuery(fieldId));
        return Ok(new
        {
            success = true,
            message = "OK",
            data = result,
            errors = Array.Empty<string>()
        });
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
    public async Task<IActionResult> GetSlotById(Guid id)
    {
        _logger.LogInformation("Fetching time slot {SlotId}", id);
        try
        {
            var result = await _mediator.Send(new CourtManager.Application.Features.TimeSlots.GetTimeSlotByIdQuery(id));
            return Ok(new
            {
                success = true,
                message = "OK",
                data = result,
                errors = Array.Empty<string>()
            });
        }
        catch (CourtManager.Application.Exceptions.NotFoundException ex)
        {
            return NotFound(new { success = false, message = ex.Message });
        }
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
        var targetDate = DateTime.SpecifyKind(date, DateTimeKind.Utc);
        var query = new CourtManager.Application.Features.TimeSlots.Queries.GetAvailableSlotsQuery(fieldId, targetDate);
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
            _logger.LogError(ex, "Unexpected error while locking slot {SlotId}: {Error}", id, ex.Message);
            return BadRequest(new
            {
                success = false,
                message = $"Lock failed: {ex.Message}",
                errors = new[] { ex.GetType().Name }
            });
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
            _logger.LogError(ex, "Unexpected error while unlocking slot {SlotId}", id);
            return BadRequest(new
            {
                success = false,
                message = "An unexpected error occurred while unlocking the slot. Please try again.",
                errors = Array.Empty<string>()
            });
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
    public async Task<IActionResult> CreateSlot([FromBody] TimeSlotDto slot)
    {
        _logger.LogInformation("Creating new time slot for field {FieldId}", slot.FieldId);
        try
        {
            var command = new CourtManager.Application.Features.TimeSlots.CreateTimeSlotCommand(CurrentUserId, slot);
            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetSlotById), new { id = result.SlotId }, new
            {
                success = true,
                message = "Slot created successfully",
                data = result,
                errors = Array.Empty<string>()
            });
        }
        catch (CourtManager.Application.Exceptions.ValidationException ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while creating slot for field {FieldId}", slot.FieldId);
            return BadRequest(new
            {
                success = false,
                message = "An unexpected error occurred while creating the slot. Please try again.",
                errors = Array.Empty<string>()
            });
        }
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
    public async Task<IActionResult> UpdateSlot(Guid id, [FromBody] TimeSlotDto slot)
    {
        _logger.LogInformation("Updating time slot {SlotId}", id);
        try
        {
            var command = new CourtManager.Application.Features.TimeSlots.UpdateTimeSlotCommand(CurrentUserId, id, slot);
            var result = await _mediator.Send(command);
            return Ok(new
            {
                success = true,
                message = "Slot updated successfully",
                data = result,
                errors = Array.Empty<string>()
            });
        }
        catch (CourtManager.Application.Exceptions.NotFoundException ex)
        {
            return NotFound(new { success = false, message = ex.Message });
        }
        catch (CourtManager.Application.Exceptions.ValidationException ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while updating slot {SlotId}", id);
            return BadRequest(new
            {
                success = false,
                message = "An unexpected error occurred while updating the slot. Please try again.",
                errors = Array.Empty<string>()
            });
        }
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
