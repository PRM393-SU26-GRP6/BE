using CourtManager.Application.DTOs;
using CourtManager.Application.Features.Fields.Commands;
using CourtManager.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CourtManager.APIs.Controllers;

/// <summary>
/// Owner-facing football field management endpoints.
/// Route: /api/v1/owner/fields
/// </summary>
[Route("api/v1/owner/fields")]
[Authorize(Roles = "Owner")]
public class OwnerFieldsController : BaseApiController
{
    private readonly IMediator _mediator;

    public OwnerFieldsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Updates a field's name, type, and price.
    /// Throws NotFoundException (404) or ForbiddenException (403) via global middleware.
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateField(Guid id, [FromBody] UpdateFieldRequestDto request)
    {
        var command = new UpdateFieldCommand(
            id,
            CurrentUserId,
            request.FieldName,
            request.FieldType,
            request.PricePerHour
        );

        var result = await _mediator.Send(command);
        return Ok(new { success = true, message = "Field updated successfully", data = result, errors = Array.Empty<string>() });
    }

    /// <summary>
    /// Toggles a field active/inactive.
    /// Throws NotFoundException (404) or ForbiddenException (403) via global middleware.
    /// </summary>
    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdateFieldStatus(Guid id, [FromBody] UpdateFieldStatusRequestDto request)
    {
        var command = new UpdateFieldStatusCommand(id, CurrentUserId, request.IsActive);
        var result = await _mediator.Send(command);

        return Ok(new { success = true, message = result.Message, data = new { isActive = result.IsActive }, errors = Array.Empty<string>() });
    }
}
