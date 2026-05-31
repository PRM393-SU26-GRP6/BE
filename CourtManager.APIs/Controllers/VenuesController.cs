using CourtManager.Application.DTOs;
using CourtManager.Application.Features.Venues.Commands;
using CourtManager.Application.Features.Venues.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CourtManager.APIs.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class VenuesController : ControllerBase
{
    private readonly IMediator _mediator;

    public VenuesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetVenues([FromQuery] GetVenuesQuery query)
    {
        var result = await _mediator.Send(query);
        
        return Ok(new
        {
            success = true,
            message = "OK",
            data = result,
            errors = Array.Empty<string>()
        });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetVenueById(Guid id)
    {
        var result = await _mediator.Send(new GetVenueByIdQuery(id));
        
        if (result == null)
        {
            return NotFound(new
            {
                success = false,
                message = "Venue not found.",
                errors = new[] { "VENUE_NOT_FOUND" }
            });
        }

        return Ok(new
        {
            success = true,
            message = "OK",
            data = result,
            errors = Array.Empty<string>()
        });
    }

    [HttpGet("{id}/fields")]
    public async Task<IActionResult> GetVenueFields(Guid id)
    {
        var result = await _mediator.Send(new GetVenueFieldsQuery(id));
        
        return Ok(new
        {
            success = true,
            message = "OK",
            data = result,
            errors = Array.Empty<string>()
        });
    }

    [HttpGet("{id}/amenities")]
    public async Task<IActionResult> GetVenueAmenities(Guid id)
    {
        var result = await _mediator.Send(new GetVenueAmenitiesQuery(id));
        
        return Ok(new
        {
            success = true,
            message = "OK",
            data = result,
            errors = Array.Empty<string>()
        });
    }

    [HttpGet("{id}/images")]
    public async Task<IActionResult> GetVenueImages(Guid id)
    {
        var result = await _mediator.Send(new GetVenueImagesQuery(id));
        
        return Ok(new
        {
            success = true,
            message = "OK",
            data = result,
            errors = Array.Empty<string>()
        });
    }

    [HttpGet("search")]
    public async Task<IActionResult> SearchVenues([FromQuery] string q, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var query = new GetVenuesQuery
        {
            Q = q,
            Page = page,
            PageSize = pageSize
        };

        var result = await _mediator.Send(query);
        
        return Ok(new
        {
            success = true,
            message = "OK",
            data = result,
            errors = Array.Empty<string>()
        });
    }

    [HttpGet("map/nearby")]
    public async Task<IActionResult> GetNearbyVenues([FromQuery] double lat, [FromQuery] double lng, [FromQuery] double radius = 5.0)
    {
        var query = new GetNearbyVenuesQuery
        {
            Latitude = lat,
            Longitude = lng,
            RadiusInKm = radius
        };

        var result = await _mediator.Send(query);
        
        return Ok(new
        {
            success = true,
            message = "OK",
            data = result,
            errors = Array.Empty<string>()
        });
    }
}
