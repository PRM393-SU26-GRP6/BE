using MediatR;
using Microsoft.AspNetCore.Identity;
using CourtManager.Application.DTOs;
using CourtManager.Domain.Entities;

namespace CourtManager.Application.Features.Auth.Commands;

public class LogoutCommandHandler : IRequestHandler<LogoutCommand, AuthResponseDto>
{
    private readonly UserManager<User> _userManager;
    private readonly CourtManager.Application.Interfaces.ICurrentUserService _currentUserService;

    public LogoutCommandHandler(UserManager<User> userManager, CourtManager.Application.Interfaces.ICurrentUserService currentUserService)
    {
        _userManager = userManager;
        _currentUserService = currentUserService;
    }

    public async Task<AuthResponseDto> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        if (userId == Guid.Empty)
        {
            return new AuthResponseDto { Success = false, Message = "Invalid user token" };
        }

        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
        {
            return new AuthResponseDto
            {
                Success = false,
                Message = "User not found"
            };
        }

        user.RefreshToken = null;
        user.RefreshTokenExpiryTime = null;
        await _userManager.UpdateAsync(user);

        return new AuthResponseDto
        {
            Success = true,
            Message = "Logged out successfully"
        };
    }
}
