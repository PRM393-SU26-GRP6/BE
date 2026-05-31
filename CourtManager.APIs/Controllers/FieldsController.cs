using CourtManager.Application.Features.FootballFields.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CourtManager.APIs.Controllers;

[ApiController]
[Route("api/v1/fields")]
public class FieldsController : ControllerBase
{
    private readonly IMediator _mediator;

    public FieldsController(IMediator mediator)
    {
        _mediator = mediator;
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
            return NotFound(new
            {
                success = false,
                message = "Field not found",
                errors = new[] { ex.Message }
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
            return BadRequest(new
            {
                success = false,
                message = "Failed to get field slots",
                errors = new[] { ex.Message }
            });
        }
    }
}
