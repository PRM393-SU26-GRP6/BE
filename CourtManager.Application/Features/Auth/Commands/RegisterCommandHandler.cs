using MediatR;
using CourtManager.Application.DTOs;
using CourtManager.Application.Interfaces;
using CourtManager.Domain.Entities;

namespace CourtManager.Application.Features.Auth.Commands;

/// <summary>
/// Handler for RegisterCommand - creates a new user account.
/// </summary>
public class RegisterCommandHandler : IRequestHandler<RegisterCommand, AuthResponseDto>
{
    private readonly IUserAuthService _userAuthService;
    private readonly IJwtTokenService _jwtTokenService;

    public RegisterCommandHandler(
        IUserAuthService userAuthService,
        IJwtTokenService jwtTokenService)
    {
        _userAuthService = userAuthService;
        _jwtTokenService = jwtTokenService;
    }

    /// <summary>
    /// Handles user registration.
    /// </summary>
    public async Task<AuthResponseDto> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        // Check if user already exists
        var existingUser = await _userAuthService.FindByEmailAsync(request.Email);
        if (existingUser != null)
        {
            return new AuthResponseDto
            {
                Success = false,
                Message = "User with this email already exists"
            };
        }

        // Check if phone number already exists
        var phoneExists = _userAuthService.Users
            .Any(u => u.PhoneNumber == request.PhoneNumber);
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
        var (succeeded, errors) = await _userAuthService.CreateAsync(user, request.Password);

        if (!succeeded)
        {
            return new AuthResponseDto
            {
                Success = false,
                Message = "Failed to register user: " + string.Join(", ", errors)
            };
        }

        // Add default role "User"
        await _userAuthService.AddToRoleAsync(user, "User");

        // Get user roles
        var roles = await _userAuthService.GetRolesAsync(user);

        // Generate tokens
        var accessToken = _jwtTokenService.GenerateAccessToken(user, roles);
        var refreshToken = _jwtTokenService.GenerateRefreshTokenJwt(user, roles);
        var refreshTokenExpiryTime = _jwtTokenService.GetRefreshTokenExpiryTime();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = refreshTokenExpiryTime;

        await _userAuthService.UpdateAsync(user);

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
