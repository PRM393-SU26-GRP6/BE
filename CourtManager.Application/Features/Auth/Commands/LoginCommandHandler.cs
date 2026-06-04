using MediatR;
using Microsoft.AspNetCore.Identity;
using CourtManager.Application.DTOs;
using CourtManager.Application.Services;
using CourtManager.Domain.Entities;

namespace CourtManager.Application.Features.Auth.Commands;

/// <summary>
/// Handler for LoginCommand - authenticates user and returns tokens.
/// </summary>
public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResponseDto>
{
    private readonly UserManager<User> _userManager;
    private readonly IJwtTokenService _jwtTokenService;

    public LoginCommandHandler(
        UserManager<User> userManager,
        IJwtTokenService jwtTokenService)
    {
        _userManager = userManager;
        _jwtTokenService = jwtTokenService;
    }

    /// <summary>
    /// Handles user login.
    /// </summary>
    public async Task<AuthResponseDto> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        // Find user by email
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
        {
            return new AuthResponseDto
            {
                Success = false,
                Message = "Invalid email or password"
            };
        }

        // Check if user is active
        if (!user.IsActive)
        {
            return new AuthResponseDto
            {
                Success = false,
                Message = "User account is inactive"
            };
        }

        // Verify password
        if (!await _userManager.CheckPasswordAsync(user, request.Password))
        {
            return new AuthResponseDto
            {
                Success = false,
                Message = "Invalid email or password"
            };
        }

        // Get roles
        var roles = await _userManager.GetRolesAsync(user);

        // Generate tokens
        var accessToken = _jwtTokenService.GenerateAccessToken(user, roles);
        var refreshToken = _jwtTokenService.GenerateRefreshTokenJwt(user, roles);
        var refreshTokenExpiryTime = _jwtTokenService.GetRefreshTokenExpiryTime();

        // Update user's refresh token
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = refreshTokenExpiryTime;
        user.UpdatedAt = DateTime.UtcNow;

        await _userManager.UpdateAsync(user);

        return new AuthResponseDto
        {
            Success = true,
            Message = "Login successful",
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            User = new UserAuthDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email!,
                PhoneNumber = user.PhoneNumber ?? string.Empty,
                Roles = roles
            }
        };
    }
}
