using MediatR;
using Microsoft.AspNetCore.Identity;
using CourtManager.Application.DTOs;
using CourtManager.Application.Interfaces;
using CourtManager.Domain.Entities;

namespace CourtManager.Application.Features.Auth.Commands;

public class VerifyOtpCommandHandler : IRequestHandler<VerifyOtpCommand, AuthResponseDto>
{
    private readonly IJwtTokenService _jwtTokenService;

    public VerifyOtpCommandHandler(
        UserManager<User> userManager,
        IJwtTokenService jwtTokenService)
    {
        _userManager = userManager;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<AuthResponseDto> Handle(VerifyOtpCommand request, CancellationToken cancellationToken)
    {
        // Find user by Email
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
        {
            return new AuthResponseDto
            {
                Success = false,
                Message = "User not found."
            };
        }

        if (string.IsNullOrEmpty(user.OtpCode) || user.OtpExpiryTime == null || user.OtpExpiryTime < DateTime.UtcNow)
        {
            return new AuthResponseDto
            {
                Success = false,
                Message = "OTP has expired or does not exist."
            };
        }

        user.OtpAttempts++;

        // Verify OTP
        if (user.OtpCode != request.Otp)
        {
            if (user.OtpAttempts >= 3)
            {
                user.OtpCode = null;
                user.OtpExpiryTime = null;
                user.OtpAttempts = 0;
                await _userManager.UpdateAsync(user);

                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Too many failed attempts. This OTP code has been invalidated. Please register again to get a new code."
                };
            }

            await _userManager.UpdateAsync(user);

            return new AuthResponseDto
            {
                Success = false,
                Message = $"Invalid OTP code. You have {3 - user.OtpAttempts} attempt(s) remaining."
            };
        }

        // Mark Email as confirmed
        user.EmailConfirmed = true;
        user.OtpCode = null;
        user.OtpExpiryTime = null;
        user.OtpAttempts = 0;
        
        var updateResult = await _userManager.UpdateAsync(user);
        if (!updateResult.Succeeded)
        {
            return new AuthResponseDto
            {
                Success = false,
                Message = "Failed to confirm user: " + string.Join(", ", updateResult.Errors.Select(e => e.Description))
            };
        }

        // Get user roles
        var roles = await _userManager.GetRolesAsync(user);

        // Generate tokens
        var accessToken = _jwtTokenService.GenerateAccessToken(user, roles);
        var refreshToken = _jwtTokenService.GenerateRefreshTokenJwt(user, roles);
        var refreshTokenExpiryTime = _jwtTokenService.GetRefreshTokenExpiryTime();

        // Save refresh token
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = refreshTokenExpiryTime;
        user.UpdatedAt = DateTime.UtcNow;

        await _userManager.UpdateAsync(user);

        return new AuthResponseDto
        {
            Success = true,
            Message = "Verification successful. Login complete.",
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
