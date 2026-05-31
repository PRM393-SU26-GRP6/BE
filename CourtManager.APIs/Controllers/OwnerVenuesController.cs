using CourtManager.Application.DTOs;
using CourtManager.Application.Features.Venues.Commands;
using CourtManager.Application.Features.Venues.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CourtManager.APIs.Controllers;

/// <summary>
/// Owner-facing venue management endpoints.
/// </summary>
[Route("api/v1/owner/venues")]
[Authorize(Roles = "Owner")]
public class OwnerVenuesController : BaseApiController
{
    private readonly IMediator _mediator;

    public OwnerVenuesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Gets all venues belonging to the logged-in owner.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetMyVenues(
        [FromQuery] bool? isActive,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var query = new GetOwnerVenuesQuery
        {
            OwnerId = CurrentUserId,
            IsActive = isActive,
            Page = page,
            PageSize = pageSize
        };

        var result = await _mediator.Send(query);
        return Ok(new { success = true, message = "OK", data = result, errors = Array.Empty<string>() });
    }

    /// <summary>
    /// Creates a new venue for the logged-in owner.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateVenue([FromBody] CreateVenueRequestDto request)
    {
        var command = new CreateVenueCommand(
            CurrentUserId,
            request.VenueName, request.Address,
            request.Latitude, request.Longitude,
            request.Description, request.OpeningHours, request.PhoneContact
        );

        var result = await _mediator.Send(command);
        return Ok(new { success = true, message = "Venue created successfully", data = result, errors = Array.Empty<string>() });
    }

    /// <summary>
    /// Uploads images for a venue. Throws ForbiddenException (403) if caller does not own the venue.
    /// </summary>
    [HttpPost("{id}/images")]
    public async Task<IActionResult> UploadVenueImages(Guid id, [FromForm] List<Microsoft.AspNetCore.Http.IFormFile> images)
    {
        if (images == null || !images.Any())
            return BadRequest(new { success = false, message = "No images provided" });

        var fileDtos = images
            .Select(f => new FileUploadDto(f.OpenReadStream(), f.FileName, f.ContentType))
            .ToList();

        var command = new UploadVenueImagesCommand(id, CurrentUserId, fileDtos);
        var uploadedUrls = await _mediator.Send(command);

        return Ok(new { success = true, message = "Images uploaded successfully", data = uploadedUrls, errors = Array.Empty<string>() });
    }

    /// <summary>
    /// Updates venue info. Throws NotFoundException (404) or ForbiddenException (403) via global middleware.
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateVenue(Guid id, [FromBody] UpdateVenueRequestDto request)
    {
        var command = new UpdateVenueCommand(
            id, CurrentUserId,
            request.VenueName, request.Address,
            request.Latitude, request.Longitude,
            request.Description, request.OpeningHours, request.PhoneContact
        );

        var result = await _mediator.Send(command);
        return Ok(new { success = true, message = "Venue updated successfully", data = result, errors = Array.Empty<string>() });
    }

    /// <summary>
    /// Toggles venue active/inactive. Blocks deactivation when active bookings exist (400).
    /// Throws NotFoundException (404) or ForbiddenException (403) via global middleware.
    /// </summary>
    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdateVenueStatus(Guid id, [FromBody] UpdateVenueStatusRequestDto request)
    {
        var command = new UpdateVenueStatusCommand(id, CurrentUserId, request.IsActive);
        var result = await _mediator.Send(command);

        return Ok(new { success = true, message = result.Message, data = new { isActive = result.IsActive }, errors = Array.Empty<string>() });
    }

    /// <summary>
    /// Deletes a venue image. Throws NotFoundException (404) or ForbiddenException (403) via global middleware.
    /// </summary>
    [HttpDelete("{id}/images/{imageId}")]
    public async Task<IActionResult> DeleteVenueImage(Guid id, Guid imageId)
    {
        var command = new DeleteVenueImageCommand(id, imageId, CurrentUserId);
        await _mediator.Send(command);

        return Ok(new { success = true, message = "Image deleted successfully", data = new { }, errors = Array.Empty<string>() });
    }
}
