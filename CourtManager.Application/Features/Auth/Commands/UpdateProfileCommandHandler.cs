using AutoMapper;
using CourtManager.Application.DTOs;
using CourtManager.Application.Interfaces;
using CourtManager.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace CourtManager.Application.Features.Auth.Commands;

public class UpdateProfileCommandHandler : IRequestHandler<UpdateProfileCommand, UserDto>
{
    private readonly UserManager<User> _userManager;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUserService;

    public UpdateProfileCommandHandler(
        UserManager<User> userManager,
        IMapper mapper,
        ICurrentUserService currentUserService)
    {
        _userManager = userManager;
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

        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null || !user.IsActive)
        {
            throw new UnauthorizedAccessException("User not found or inactive.");
        }

        user.FullName = request.FullName;
        user.Phone = request.Phone;
        user.PhoneNumber = request.Phone; // keep Identity field in sync
        user.AvatarUrl = request.AvatarUrl;
        user.UpdatedAt = DateTime.UtcNow;

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            throw new InvalidOperationException(
                "Failed to update profile: " + string.Join(", ", result.Errors.Select(e => e.Description)));
        }

        return _mapper.Map<UserDto>(user);
    }
}
