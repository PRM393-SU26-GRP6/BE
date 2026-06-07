using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using CourtManager.Application.Interfaces;
using CourtManager.Domain.Entities;
using CourtManager.Infrastructure;

namespace CourtManager.Application.Features.Auth.Commands;

public class SendOtpCommandHandler : IRequestHandler<SendOtpCommand, SendOtpResponseDto>
{
    private readonly UserManager<User> _userManager;
    private readonly ApplicationDbContext _dbContext;
    private readonly IEmailService _emailService;

    public SendOtpCommandHandler(
        UserManager<User> userManager,
        ApplicationDbContext dbContext,
        IEmailService emailService)
    {
        _userManager = userManager;
        _dbContext = dbContext;
        _emailService = emailService;
    }

    public async Task<SendOtpResponseDto> Handle(SendOtpCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
        {
            return new SendOtpResponseDto
            {
                Success = false,
                Message = "Email not found"
            };
        }

        var otp = GenerateOtp();
        var now = DateTime.UtcNow;

        var token = new EmailVerificationToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Token = otp,
            Email = request.Email,
            CreatedAt = now,
            ExpiresAt = now.AddMinutes(10),
            IsUsed = false
        };

        _dbContext.EmailVerificationTokens.Add(token);
        await _dbContext.SaveChangesAsync(cancellationToken);

        var emailSent = await _emailService.SendOtpAsync(request.Email, otp, cancellationToken);
        if (!emailSent)
        {
            return new SendOtpResponseDto
            {
                Success = false,
                Message = "Failed to send OTP email"
            };
        }

        return new SendOtpResponseDto
        {
            Success = true,
            Message = "OTP sent to your email"
        };
    }

    private static string GenerateOtp()
    {
        var random = new Random();
        return random.Next(100000, 999999).ToString();
    }
}
