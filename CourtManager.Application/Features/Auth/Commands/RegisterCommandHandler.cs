using MediatR;
using Microsoft.AspNetCore.Identity;
using CourtManager.Application.DTOs;
using CourtManager.Application.Services;
using CourtManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CourtManager.Application.Features.Auth.Commands;

/// <summary>
/// Handler for RegisterCommand - creates a new user account.
/// </summary>
public class RegisterCommandHandler : IRequestHandler<RegisterCommand, AuthResponseDto>
{
    private readonly UserManager<User> _userManager;
    private readonly IJwtTokenService _jwtTokenService;

    public RegisterCommandHandler(
        UserManager<User> userManager,
        IJwtTokenService jwtTokenService)
    {
        _userManager = userManager;
        _jwtTokenService = jwtTokenService;
    }

    /// <summary>
    /// Handles user registration.
    /// </summary>
    public async Task<AuthResponseDto> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        // Check if user already exists
        var existingUser = await _userManager.FindByEmailAsync(request.Email);
        if (existingUser != null)
        {
            return new AuthResponseDto
            {
                Success = false,
                Message = "User with this email already exists"
            };
        }

        // Check if phone number already exists
        var phoneExists = await _userManager.Users.AnyAsync(u => u.PhoneNumber == request.PhoneNumber, cancellationToken);
        if (phoneExists)
        {
            return new AuthResponseDto
            {
                Success = false,
                Message = "User with this phone number already exists"
            };
        }

        // Create new user
        var user = new User
        {
            UserName = request.Email,
            Email = request.Email,
            FullName = request.FullName,
            Phone = request.PhoneNumber,
            PhoneNumber = request.PhoneNumber,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        // Save user to database
        var result = await _userManager.CreateAsync(user, request.Password);
        
        if (!result.Succeeded)
        {
            return new AuthResponseDto
            {
                Success = false,
                Message = "Failed to register user: " + string.Join(", ", result.Errors.Select(e => e.Description))
            };
        }

        // Add default role "User"
        await _userManager.AddToRoleAsync(user, "User");

        // Get user roles
        var roles = await _userManager.GetRolesAsync(user);

        // Generate tokens
        var accessToken = _jwtTokenService.GenerateAccessToken(user, roles);
        var refreshToken = _jwtTokenService.GenerateRefreshTokenJwt(user, roles);
        var refreshTokenExpiryTime = _jwtTokenService.GetRefreshTokenExpiryTime();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = refreshTokenExpiryTime;

        await _userManager.UpdateAsync(user);

        return new AuthResponseDto
        {
            Success = true,
            Message = "User registered successfully",
            AccessToken = accessToken,
            RefreshToken = refreshToken,
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
