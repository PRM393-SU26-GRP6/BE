using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using CourtManager.Application.DTOs;
using CourtManager.Domain.Entities;
using CourtManager.Infrastructure;

namespace CourtManager.Application.Features.Auth.Commands;

public class VerifyOtpCommandHandler : IRequestHandler<VerifyOtpCommand, AuthResponseDto>
{
    private readonly UserManager<User> _userManager;
    private readonly ApplicationDbContext _dbContext;

    public VerifyOtpCommandHandler(
        UserManager<User> userManager,
        ApplicationDbContext dbContext)
    {
        _userManager = userManager;
        _dbContext = dbContext;
    }

    public async Task<AuthResponseDto> Handle(VerifyOtpCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
        {
            return new AuthResponseDto
            {
                Success = false,
                Message = "Email not found"
            };
        }

        var token = await _dbContext.EmailVerificationTokens
            .FirstOrDefaultAsync(t =>
                t.Email == request.Email &&
                t.Token == request.Otp &&
                !t.IsUsed &&
                t.ExpiresAt > DateTime.UtcNow,
                cancellationToken);

        if (token == null)
        {
            return new AuthResponseDto
            {
                Success = false,
                Message = "Invalid or expired OTP"
            };
        }

        user.IsEmailVerified = true;
        user.EmailVerifiedAt = DateTime.UtcNow;

        token.IsUsed = true;
        token.UsedAt = DateTime.UtcNow;

        await _userManager.UpdateAsync(user);
        _dbContext.EmailVerificationTokens.Update(token);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new AuthResponseDto
        {
            Success = true,
            Message = "Email verified successfully"
        };
    }
}
