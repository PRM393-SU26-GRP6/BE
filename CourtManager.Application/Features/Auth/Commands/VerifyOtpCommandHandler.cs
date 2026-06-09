using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using CourtManager.Application.DTOs;
using CourtManager.Application.Interfaces;
using CourtManager.Domain.Entities;

namespace CourtManager.Application.Features.Auth.Commands;

public class VerifyOtpCommandHandler : IRequestHandler<VerifyOtpCommand, AuthResponseDto>
{
    private readonly UserManager<User> _userManager;
    private readonly IMemoryCache _memoryCache;
    private readonly IJwtTokenService _jwtTokenService;

    public VerifyOtpCommandHandler(
        UserManager<User> userManager,
        IMemoryCache memoryCache,
        IJwtTokenService jwtTokenService)
    {
        _userManager = userManager;
        _memoryCache = memoryCache;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<AuthResponseDto> Handle(VerifyOtpCommand request, CancellationToken cancellationToken)
    {
        var cacheKey = $"otp:register:{request.Email}";
        var attemptsKey = $"otp:attempts:{request.Email}";

        // Retrieve OTP code from IMemoryCache
        if (!_memoryCache.TryGetValue(cacheKey, out string? cachedOtp) || string.IsNullOrEmpty(cachedOtp))
        {
            return new AuthResponseDto
            {
                Success = false,
                Message = "OTP has expired or does not exist."
            };
        }

        // Verify OTP
        if (cachedOtp != request.Otp)
        {
            _memoryCache.TryGetValue(attemptsKey, out int attempts);
            attempts++;

            if (attempts >= 3)
            {
                // Invalidate OTP and attempts counter
                _memoryCache.Remove(cacheKey);
                _memoryCache.Remove(attemptsKey);

                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Too many failed attempts. This OTP code has been invalidated. Please register again to get a new code."
                };
            }

            // Save attempts count back to cache for 10 minutes
            _memoryCache.Set(attemptsKey, attempts, TimeSpan.FromMinutes(10));

            return new AuthResponseDto
            {
                Success = false,
                Message = $"Invalid OTP code. You have {3 - attempts} attempt(s) remaining."
            };
        }

        // Clean up cache keys on successful validation
        _memoryCache.Remove(cacheKey);
        _memoryCache.Remove(attemptsKey);

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

        // Mark Email as confirmed
        user.EmailConfirmed = true;
        
        var updateResult = await _userManager.UpdateAsync(user);
        if (!updateResult.Succeeded)
        {
            return new AuthResponseDto
            {
                Success = false,
                Message = "Failed to confirm user: " + string.Join(", ", updateResult.Errors.Select(e => e.Description))
            };
        }

        // Clear OTP code from cache
        _memoryCache.Remove(cacheKey);

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
