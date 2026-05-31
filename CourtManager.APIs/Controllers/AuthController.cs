using Microsoft.AspNetCore.Mvc;
using MediatR;
using CourtManager.Application.DTOs;
using CourtManager.Application.Features.Auth.Commands;
using System.Security.Claims;

namespace CourtManager.APIs.Controllers;

/// <summary>
/// Controller for user authentication endpoints (register, login, refresh token).
/// </summary>
[ApiController]
[Route("api/v1/auth")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get current logged-in user profile.
    /// </summary>
    [NonAction]
    [Microsoft.AspNetCore.Authorization.Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<UserDto>> GetMe()
    {
        // Notice how we don't need to extract the UserId from Claims here anymore!
        // The GetMeQuery is empty, the Handler uses ICurrentUserService to get the UserId.
        var result = await _mediator.Send(new CourtManager.Application.Features.Auth.Queries.GetMeQuery());
        return Ok(result);
    }

    /// <summary>
    /// Register a new user account.
    /// </summary>
    [HttpPost("register")]
    [Microsoft.AspNetCore.RateLimiting.EnableRateLimiting("AuthPolicy")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AuthResponseDto>> Register([FromBody] RegisterRequestDto request)
    {
        if (request.Password != request.ConfirmPassword)
        {
            return BadRequest(new AuthResponseDto { Success = false, Message = "Passwords do not match" });
        }

        var command = new RegisterCommand
        {
            FullName = request.FullName,
            Email = request.Email,
            PhoneNumber = string.IsNullOrWhiteSpace(request.PhoneNumber) ? request.Phone : request.PhoneNumber,
            Password = request.Password,
            ConfirmPassword = request.ConfirmPassword,
            Role = request.Role
        };

        var result = await _mediator.Send(command);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Login user and return access and refresh tokens.
    /// </summary>
    [HttpPost("login")]
    [Microsoft.AspNetCore.RateLimiting.EnableRateLimiting("AuthPolicy")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginRequestDto request)
    {
        var command = new LoginCommand
        {
            Email = request.Email,
            Password = request.Password
        };

        var result = await _mediator.Send(command);

        if (!result.Success)
        {
            return Unauthorized(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Refresh access token using refresh token.
    /// </summary>
    [HttpPost("refresh-token")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AuthResponseDto>> RefreshToken([FromBody] RefreshTokenRequestDto request)
    {
        // Get the access token from Authorization header
        var authHeader = Request.Headers["Authorization"].ToString();
        var accessToken = authHeader.StartsWith("Bearer ") ? authHeader.Substring("Bearer ".Length) : string.Empty;

        if (string.IsNullOrEmpty(accessToken))
        {
            return BadRequest(new AuthResponseDto
            {
                Success = false,
                Message = "Access token is required"
            });
        }

        var command = new RefreshTokenCommand
        {
            AccessToken = accessToken,
            RefreshToken = request.RefreshToken
        };

        var result = await _mediator.Send(command);

        if (!result.Success)
        {
            return Unauthorized(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Logout user by revoking refresh token.
    /// </summary>
    [HttpPost("logout")]
    [Microsoft.AspNetCore.Authorization.Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthResponseDto>> Logout()
    {
        var command = new LogoutCommand();
        var result = await _mediator.Send(command);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Change current user's password.
    /// </summary>
    [NonAction]
    [Microsoft.AspNetCore.Authorization.Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthResponseDto>> ChangePassword([FromBody] ChangePasswordRequestDto request)
    {
        if (request.NewPassword != request.ConfirmNewPassword)
        {
            return BadRequest(new AuthResponseDto { Success = false, Message = "New passwords do not match" });
        }

        var command = new ChangePasswordCommand
        {
            CurrentPassword = request.CurrentPassword,
            NewPassword = request.NewPassword
        };

        var result = await _mediator.Send(command);
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    /// <summary>
    /// Request a password reset token.
    /// </summary>
    [NonAction]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AuthResponseDto>> ForgotPassword([FromBody] ForgotPasswordRequestDto request)
    {
        var command = new ForgotPasswordCommand { Email = request.Email };
        var result = await _mediator.Send(command);
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    /// <summary>
    /// Reset password using a reset token.
    /// </summary>
    [NonAction]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AuthResponseDto>> ResetPassword([FromBody] ResetPasswordRequestDto request)
    {
        if (request.NewPassword != request.ConfirmNewPassword)
        {
            return BadRequest(new AuthResponseDto { Success = false, Message = "New passwords do not match" });
        }

        var command = new ResetPasswordCommand
        {
            Email = request.Email,
            Token = request.Token,
            NewPassword = request.NewPassword
        };

        var result = await _mediator.Send(command);
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }
}
