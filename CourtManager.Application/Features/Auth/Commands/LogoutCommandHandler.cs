using MediatR;
using CourtManager.Application.DTOs;
using CourtManager.Application.Interfaces;

namespace CourtManager.Application.Features.Auth.Commands;

public class LogoutCommandHandler : IRequestHandler<LogoutCommand, AuthResponseDto>
{
    private readonly IUserAuthService _userAuthService;
    private readonly ICurrentUserService _currentUserService;

    public LogoutCommandHandler(IUserAuthService userAuthService, ICurrentUserService currentUserService)
    {
        _userAuthService = userAuthService;
        _currentUserService = currentUserService;
    }

    public async Task<AuthResponseDto> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        if (userId == Guid.Empty)
        {
            return new AuthResponseDto { Success = false, Message = "Invalid user token" };
        }

        var user = await _userAuthService.FindByIdAsync(userId);
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
        await _userAuthService.UpdateAsync(user);

        return new AuthResponseDto
        {
            Success = true,
            Message = "Logged out successfully"
        };
    }
}
