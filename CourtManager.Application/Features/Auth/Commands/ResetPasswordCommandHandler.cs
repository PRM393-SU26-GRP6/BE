using MediatR;
using Microsoft.AspNetCore.Identity;
using CourtManager.Application.DTOs;
using CourtManager.Domain.Entities;

namespace CourtManager.Application.Features.Auth.Commands;

public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, AuthResponseDto>
{
    private readonly UserManager<User> _userManager;

    public ResetPasswordCommandHandler(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    public async Task<AuthResponseDto> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
        {
            return new AuthResponseDto { Success = false, Message = "Invalid email or token." };
        }

        var result = await _userManager.ResetPasswordAsync(user, request.Token, request.NewPassword);

        if (!result.Succeeded)
        {
            return new AuthResponseDto 
            { 
                Success = false, 
                Message = "Failed to reset password: " + string.Join(", ", result.Errors.Select(e => e.Description)) 
            };
        }

        return new AuthResponseDto { Success = true, Message = "Password has been reset successfully." };
    }
}
