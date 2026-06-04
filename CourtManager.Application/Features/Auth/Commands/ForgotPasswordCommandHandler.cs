using MediatR;
using CourtManager.Application.DTOs;
using CourtManager.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace CourtManager.Application.Features.Auth.Commands;

public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand, AuthResponseDto>
{
    private readonly IUserAuthService _userAuthService;
    private readonly ILogger<ForgotPasswordCommandHandler> _logger;

    public ForgotPasswordCommandHandler(IUserAuthService userAuthService, ILogger<ForgotPasswordCommandHandler> logger)
    {
        _userAuthService = userAuthService;
        _logger = logger;
    }

    public async Task<AuthResponseDto> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _userAuthService.FindByEmailAsync(request.Email);
        if (user == null || !user.IsActive)
        {
            // Do not reveal that the user does not exist or is inactive
            return new AuthResponseDto { Success = true, Message = "If the email exists, a password reset token has been sent." };
        }

        var token = await _userAuthService.GeneratePasswordResetTokenAsync(user);

        // In a real application, you would send this token via email.
        // For development/testing, we'll log it and return it in the message (optional).
        _logger.LogInformation("Password reset token for {Email}: {Token}", user.Email, token);

        return new AuthResponseDto
        {
            Success = true,
            Message = "Password reset token generated successfully. Please check your email.",
            // Returning token here just for Postman testing convenience in development
            AccessToken = token
        };
    }
}
