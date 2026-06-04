using MediatR;
using Microsoft.AspNetCore.Identity;
using CourtManager.Application.DTOs;
using CourtManager.Domain.Entities;

namespace CourtManager.Application.Features.Auth.Commands;

public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, AuthResponseDto>
{
    private readonly UserManager<User> _userManager;
    private readonly CourtManager.Application.Interfaces.ICurrentUserService _currentUserService;

    public ChangePasswordCommandHandler(UserManager<User> userManager, CourtManager.Application.Interfaces.ICurrentUserService currentUserService)
    {
        _userManager = userManager;
        _currentUserService = currentUserService;
    }

    public async Task<AuthResponseDto> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        if (userId == Guid.Empty)
        {
            return new AuthResponseDto { Success = false, Message = "Invalid user token" };
        }

        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
        {
            return new AuthResponseDto { Success = false, Message = "User not found" };
        }

        var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);

        if (!result.Succeeded)
        {
            return new AuthResponseDto 
            { 
                Success = false, 
                Message = "Failed to change password: " + string.Join(", ", result.Errors.Select(e => e.Description)) 
            };
        }

        return new AuthResponseDto { Success = true, Message = "Password changed successfully" };
    }
}
