using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Identity;
using CourtManager.Application.DTOs;
using CourtManager.Application.Services;
using CourtManager.Domain.Entities;

namespace CourtManager.Application.Features.Auth.Commands;

/// <summary>
/// Handler for RefreshTokenCommand - generates new access token using refresh token.
/// </summary>
public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, AuthResponseDto>
{
    private readonly UserManager<User> _userManager;
    private readonly IJwtTokenService _jwtTokenService;

    public RefreshTokenCommandHandler(
        UserManager<User> userManager,
        IJwtTokenService jwtTokenService)
    {
        _userManager = userManager;
        _jwtTokenService = jwtTokenService;
    }

    /// <summary>
    /// Handles refresh token request to generate new access token.
    /// </summary>
    public async Task<AuthResponseDto> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        // Get principal from expired access token
        var principal = _jwtTokenService.GetPrincipalFromExpiredToken(request.AccessToken);
        if (principal == null)
        {
            return new AuthResponseDto
            {
                Success = false,
                Message = "Invalid access token"
            };
        }

        // Get user ID from claims
        var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userIdClaim, out var userId))
        {
            return new AuthResponseDto
            {
                Success = false,
                Message = "Invalid token"
            };
        }

        // Find user
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
        {
            return new AuthResponseDto
            {
                Success = false,
                Message = "User not found"
            };
        }

        // Validate refresh token
        if (user.RefreshToken != request.RefreshToken)
        {
            return new AuthResponseDto
            {
                Success = false,
                Message = "Invalid refresh token"
            };
        }

        // Check if refresh token is expired
        if (user.RefreshTokenExpiryTime <= DateTime.UtcNow)
        {
            return new AuthResponseDto
            {
                Success = false,
                Message = "Refresh token expired"
            };
        }

        // Get roles
        var roles = await _userManager.GetRolesAsync(user);

        // Generate tokens
        var newAccessToken = _jwtTokenService.GenerateAccessToken(user, roles);
        var newRefreshToken = _jwtTokenService.GenerateRefreshTokenJwt(user, roles);
        var newRefreshTokenExpiryTime = _jwtTokenService.GetRefreshTokenExpiryTime();

        // Update user's refresh token
        user.RefreshToken = newRefreshToken;
        user.RefreshTokenExpiryTime = newRefreshTokenExpiryTime;
        user.UpdatedAt = DateTime.UtcNow;

        await _userManager.UpdateAsync(user);

        return new AuthResponseDto
        {
            Success = true,
            Message = "Token refreshed successfully",
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken,
            User = new UserAuthDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email!,
                PhoneNumber = user.PhoneNumber ?? string.Empty
            }
        };
    }
}
