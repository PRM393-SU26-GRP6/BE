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
}
