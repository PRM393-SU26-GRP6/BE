using CourtManager.Application.Features.Amenities.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CourtManager.APIs.Controllers;

[ApiController]
[Route("api/v1/amenities")]
public class AmenitiesController : ControllerBase
{
    private readonly IMediator _mediator;

    public AmenitiesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAmenities()
    {
        var result = await _mediator.Send(new GetAllAmenitiesQuery());
        return Ok(new
        {
            success = true,
            message = "OK",
            data = result,
            errors = Array.Empty<string>()
        });
    }
}
