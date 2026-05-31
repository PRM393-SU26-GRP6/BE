using CourtManager.Application.DTOs;
using CourtManager.Application.Features.Auth.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CourtManager.APIs.Controllers;

/// <summary>
/// User profile endpoints for the currently logged-in user.
/// </summary>
[Route("api/v1/users")]
[ApiController]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;

    public UsersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Updates the current user's profile (full name, phone, avatar URL).
    /// Email and password are not updated here.
    /// </summary>
    [HttpPut("profile")]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequestDto request)
    {
        var command = new UpdateProfileCommand
        {
            FullName = request.FullName,
            Phone = request.Phone,
            AvatarUrl = request.AvatarUrl
        };

        var result = await _mediator.Send(command);

        return Ok(new
        {
            success = true,
            message = "Profile updated successfully",
            data = result,
            errors = Array.Empty<string>()
        });
    }
}
