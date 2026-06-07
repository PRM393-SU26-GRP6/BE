using CourtManager.Application.Features.FootballFields.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CourtManager.APIs.Controllers;

[ApiController]
[Route("api/v1/fields")]
public class FieldsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<FieldsController> _logger;

    public FieldsController(IMediator mediator, ILogger<FieldsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetFieldById(Guid id)
    {
        try
        {
            var result = await _mediator.Send(new GetFieldByIdQuery(id));
            return Ok(new
            {
                success = true,
                message = "OK",
                data = result,
                errors = Array.Empty<string>()
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while fetching field {FieldId}", id);
            return NotFound(new
            {
                success = false,
                message = "Field not found",
                errors = Array.Empty<string>()
            });
        }
    }

    [HttpGet("{id}/slots")]
    public async Task<IActionResult> GetFieldSlots(Guid id, [FromQuery] DateTime? date)
    {
        try
        {
            var targetDate = date.HasValue ? DateTime.SpecifyKind(date.Value, DateTimeKind.Utc) : DateTime.UtcNow.Date;
            var query = new CourtManager.Application.Features.TimeSlots.Queries.GetAvailableSlotsQuery(id, targetDate);
            var result = await _mediator.Send(query);

            return Ok(new
            {
                success = true,
                message = "OK",
                data = result,
                errors = Array.Empty<string>()
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while fetching slots for field {FieldId}", id);
            return BadRequest(new
            {
                success = false,
                message = "An unexpected error occurred while fetching field slots. Please try again.",
                errors = Array.Empty<string>()
            });
        }
    }
}
