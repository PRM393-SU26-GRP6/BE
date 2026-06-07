using MediatR;
using Microsoft.AspNetCore.Identity;
using CourtManager.Application.DTOs;
using CourtManager.Application.Interfaces;
using CourtManager.Domain.Entities;

namespace CourtManager.Application.Features.Auth.Commands;

public class VerifyOtpCommandHandler : IRequestHandler<VerifyOtpCommand, AuthResponseDto>
{
    private readonly UserManager<User> _userManager;
    private readonly IEmailVerificationTokenRepository _tokenRepository;

    public VerifyOtpCommandHandler(
        UserManager<User> userManager,
        IEmailVerificationTokenRepository tokenRepository)
    {
        _userManager = userManager;
        _tokenRepository = tokenRepository;
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

        var token = await _tokenRepository.GetValidTokenAsync(request.Email, request.Otp, cancellationToken);
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
        await _tokenRepository.UpdateAsync(token, cancellationToken);

        return new AuthResponseDto
        {
            Success = true,
            Message = "Email verified successfully"
        };
    }
}
