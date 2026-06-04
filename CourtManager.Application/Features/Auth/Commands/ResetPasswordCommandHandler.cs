using MediatR;
using CourtManager.Application.DTOs;
using CourtManager.Application.Interfaces;

namespace CourtManager.Application.Features.Auth.Commands;

public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, AuthResponseDto>
{
    private readonly IUserAuthService _userAuthService;

    public ResetPasswordCommandHandler(IUserAuthService userAuthService)
    {
        _userAuthService = userAuthService;
    }

    public async Task<AuthResponseDto> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _userAuthService.FindByEmailAsync(request.Email);
        if (user == null)
        {
            return new AuthResponseDto { Success = false, Message = "Invalid email or token." };
        }

        var (succeeded, errors) = await _userAuthService.ResetPasswordAsync(user, request.Token, request.NewPassword);

        if (!succeeded)
        {
            return new AuthResponseDto
            {
                Success = false,
                Message = "Failed to reset password: " + string.Join(", ", errors)
            };
        }

        return new AuthResponseDto { Success = true, Message = "Password has been reset successfully." };
    }
}
