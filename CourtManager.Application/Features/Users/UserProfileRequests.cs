using CourtManager.Application.DTOs;
using CourtManager.Application.Exceptions;
using CourtManager.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace CourtManager.Application.Features.Users;

public record GetUserProfileQuery(Guid UserId) : IRequest<UserDto>;
public record UpdateUserProfileCommand(Guid UserId, UpdateUserProfileDto Profile) : IRequest<UserDto>;

public class GetUserProfileQueryHandler : IRequestHandler<GetUserProfileQuery, UserDto>
{
    private readonly UserManager<User> _userManager;

    public GetUserProfileQueryHandler(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    public async Task<UserDto> Handle(GetUserProfileQuery request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.UserId.ToString());
        if (user == null)
            throw new NotFoundException(nameof(User), request.UserId);

        var roles = await _userManager.GetRolesAsync(user);
        return ToDto(user, roles);
    }

    internal static UserDto ToDto(User user, IEnumerable<string> roles)
    {
        return new UserDto
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email ?? string.Empty,
            PhoneNumber = user.PhoneNumber ?? user.Phone,
            AvatarUrl = user.AvatarUrl,
            LoyaltyPoints = user.LoyaltyPoints,
            IsActive = user.IsActive,
            Roles = roles
        };
    }
}

public class UpdateUserProfileCommandHandler : IRequestHandler<UpdateUserProfileCommand, UserDto>
{
    private readonly UserManager<User> _userManager;

    public UpdateUserProfileCommandHandler(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    public async Task<UserDto> Handle(UpdateUserProfileCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.UserId.ToString());
        if (user == null)
            throw new NotFoundException(nameof(User), request.UserId);

        user.FullName = request.Profile.FullName.Trim();
        var phone = string.IsNullOrWhiteSpace(request.Profile.PhoneNumber)
            ? request.Profile.Phone
            : request.Profile.PhoneNumber;
        user.Phone = phone.Trim();
        user.PhoneNumber = phone.Trim();
        user.AvatarUrl = request.Profile.AvatarUrl;
        user.UpdatedAt = DateTime.UtcNow;

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
            throw new ValidationException(string.Join(", ", result.Errors.Select(e => e.Description)));

        var roles = await _userManager.GetRolesAsync(user);
        return GetUserProfileQueryHandler.ToDto(user, roles);
    }
}
