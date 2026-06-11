using MediatR;
using Microsoft.AspNetCore.Identity;
using CourtManager.Application.DTOs;
using CourtManager.Application.Interfaces;
using CourtManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace CourtManager.Application.Features.Auth.Commands;

/// <summary>
/// Handler for RegisterCommand - creates a new user account.
/// </summary>
public class RegisterCommandHandler : IRequestHandler<RegisterCommand, AuthResponseDto>
{
    private readonly UserManager<User> _userManager;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly Microsoft.Extensions.Caching.Memory.IMemoryCache _memoryCache;
    private readonly IEmailService _emailService;

    public RegisterCommandHandler(
        UserManager<User> userManager,
        IJwtTokenService jwtTokenService,
        Microsoft.Extensions.Caching.Memory.IMemoryCache memoryCache,
        IEmailService emailService)
    {
        _userManager = userManager;
        _jwtTokenService = jwtTokenService;
        _memoryCache = memoryCache;
        _emailService = emailService;
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

        // Generate OTP
        var otpCode = new Random().Next(100000, 999999).ToString();

        // Create new user
        var user = new User
        {
            UserName = request.Email,
            Email = request.Email,
            FullName = request.FullName,
            Phone = request.PhoneNumber,
            PhoneNumber = request.PhoneNumber,
            IsActive = true,
            EmailConfirmed = false, // Must verify via OTP
            CreatedAt = DateTime.UtcNow,
            OtpCode = otpCode,
            OtpExpiryTime = DateTime.UtcNow.AddMinutes(10),
            OtpAttempts = 0
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

        // Add role dynamically based on enum
        var roleName = request.Role.ToString();
        await _userManager.AddToRoleAsync(user, roleName);

        // OTP is already saved to the database during user creation

        // Send OTP via Gmail
        var emailSubject = "CourtManager - Account Verification OTP";
        var emailBody = $@"
            <h2>Welcome to CourtManager!</h2>
            <p>Thank you for registering. Please use the following One-Time Password (OTP) to verify your account:</p>
            <h1 style='color: #1e88e5; letter-spacing: 5px;'>{otpCode}</h1>
            <p>This OTP is valid for 10 minutes. If you did not request this, please ignore this email.</p>
            <br/>
            <p>Best regards,</p>
            <p>The CourtManager Team</p>";

        try
        {
            await _emailService.SendEmailAsync(request.Email, emailSubject, emailBody);
        }
        catch (Exception ex)
        {
            // Even if email fails, we don't block the API return, but we log the error or mention it.
            // Ideally in dev, we log it.
            System.Diagnostics.Debug.WriteLine($"Failed to send verification email: {ex.Message}");
        }

        return new AuthResponseDto
        {
            Success = true,
            Message = "User registered successfully. Please verify your email using the OTP code sent.",
            AccessToken = null, // Do not log in yet
            RefreshToken = null,
            User = new UserAuthDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email!,
                PhoneNumber = user.PhoneNumber ?? string.Empty,
                Roles = new[] { roleName }
            }
        };
    }
}
