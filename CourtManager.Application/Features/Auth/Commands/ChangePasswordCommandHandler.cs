using MediatR;
using CourtManager.Application.DTOs;
using CourtManager.Application.Interfaces;

namespace CourtManager.Application.Features.Auth.Commands;

public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, AuthResponseDto>
{
    private readonly IUserAuthService _userAuthService;
    private readonly ICurrentUserService _currentUserService;

    public ChangePasswordCommandHandler(IUserAuthService userAuthService, ICurrentUserService currentUserService)
    {
        _userAuthService = userAuthService;
        _currentUserService = currentUserService;
    }

    public async Task<AuthResponseDto> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        if (userId == Guid.Empty)
        {
            return new AuthResponseDto { Success = false, Message = "Invalid user token" };
        }

        var user = await _userAuthService.FindByIdAsync(userId);
        if (user == null)
        {
            return new AuthResponseDto { Success = false, Message = "User not found" };
        }

        var (succeeded, errors) = await _userAuthService.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);

        if (!succeeded)
        {
            return new AuthResponseDto
            {
                Success = false,
                Message = "Failed to change password: " + string.Join(", ", errors)
            };
        }

        return new AuthResponseDto { Success = true, Message = "Password changed successfully" };
    }
}
