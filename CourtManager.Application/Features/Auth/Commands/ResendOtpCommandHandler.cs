using MediatR;
using Microsoft.AspNetCore.Identity;
using CourtManager.Application.DTOs;
using CourtManager.Application.Interfaces;
using CourtManager.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace CourtManager.Application.Features.Auth.Commands;

public class ResendOtpCommandHandler : IRequestHandler<ResendOtpCommand, AuthResponseDto>
{
    private readonly UserManager<User> _userManager;
    private readonly IEmailService _emailService;
    private readonly ILogger<ResendOtpCommandHandler> _logger;

    public ResendOtpCommandHandler(
        UserManager<User> userManager,
        IEmailService emailService,
        ILogger<ResendOtpCommandHandler> logger)
    {
        _userManager = userManager;
        _emailService = emailService;
        _logger = logger;
    }

    public async Task<AuthResponseDto> Handle(ResendOtpCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
        {
            return new AuthResponseDto
            {
                Success = false,
                Message = "User not found."
            };
        }

        if (user.EmailConfirmed)
        {
            return new AuthResponseDto
            {
                Success = false,
                Message = "Email is already confirmed."
            };
        }

        // Generate new OTP
        var otpCode = new Random().Next(100000, 999999).ToString();
        user.OtpCode = otpCode;
        user.OtpExpiryTime = DateTime.UtcNow.AddMinutes(10);
        user.OtpAttempts = 0;

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            _logger.LogError("Failed to update user {Email} with new OTP.", request.Email);
            return new AuthResponseDto
            {
                Success = false,
                Message = "Failed to generate new OTP."
            };
        }

        // Send Email
        var emailSubject = "CourtManager - Account Verification OTP";
        var emailBody = $@"
            <h2>Welcome back to CourtManager!</h2>
            <p>Here is your new One-Time Password (OTP) to verify your account:</p>
            <h1 style='color: #1e88e5; letter-spacing: 5px;'>{otpCode}</h1>
            <p>This OTP is valid for 10 minutes. If you did not request this, please ignore this email.</p>
            <br/>
            <p>Best regards,</p>
            <p>The CourtManager Team</p>";

        try
        {
            await _emailService.SendEmailAsync(user.Email, emailSubject, emailBody);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send verification email for {Email}", user.Email);
        }

        return new AuthResponseDto
        {
            Success = true,
            Message = "A new OTP has been sent to your email."
        };
    }
}
