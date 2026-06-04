using AutoMapper;
using CourtManager.Application.DTOs;
using CourtManager.Application.Interfaces;
using CourtManager.Domain.Entities;
using MediatR;

namespace CourtManager.Application.Features.Auth.Commands;

public class UpdateProfileCommandHandler : IRequestHandler<UpdateProfileCommand, UserDto>
{
    private readonly IUserAuthService _userAuthService;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUserService;

    public UpdateProfileCommandHandler(
        IUserAuthService userAuthService,
        IMapper mapper,
        ICurrentUserService currentUserService)
    {
        _userAuthService = userAuthService;
        _mapper = mapper;
        _currentUserService = currentUserService;
    }

    public async Task<UserDto> Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        if (userId == Guid.Empty)
        {
            throw new UnauthorizedAccessException("Invalid token claims or user is not authenticated.");
        }

        var user = await _userAuthService.FindByIdAsync(userId);
        if (user == null || !user.IsActive)
        {
            throw new UnauthorizedAccessException("User not found or inactive.");
        }

        user.FullName = request.FullName;
        user.Phone = request.Phone;
        user.PhoneNumber = request.Phone; // keep Identity field in sync
        user.AvatarUrl = request.AvatarUrl;
        user.UpdatedAt = DateTime.UtcNow;

        var (succeeded, errors) = await _userAuthService.UpdateAsync(user);
        if (!succeeded)
        {
            throw new InvalidOperationException(
                "Failed to update profile: " + string.Join(", ", errors));
        }

        return _mapper.Map<UserDto>(user);
    }
}
