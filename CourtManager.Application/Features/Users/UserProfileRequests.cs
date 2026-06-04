using CourtManager.Application.DTOs;
using CourtManager.Application.Exceptions;
using CourtManager.Application.Interfaces;
using CourtManager.Domain.Entities;
using MediatR;

namespace CourtManager.Application.Features.Users;

public record GetUserProfileQuery(Guid UserId) : IRequest<UserDto>;
public record UpdateUserProfileCommand(Guid UserId, UpdateUserProfileDto Profile) : IRequest<UserDto>;

public class GetUserProfileQueryHandler : IRequestHandler<GetUserProfileQuery, UserDto>
{
    private readonly IUserAuthService _userAuthService;

    public GetUserProfileQueryHandler(IUserAuthService userAuthService)
    {
        _userAuthService = userAuthService;
    }

    public async Task<UserDto> Handle(GetUserProfileQuery request, CancellationToken cancellationToken)
    {
        var user = await _userAuthService.FindByIdAsync(request.UserId);
        if (user == null)
            throw new NotFoundException(nameof(User), request.UserId);

        var roles = await _userAuthService.GetRolesAsync(user);
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
    private readonly IUserAuthService _userAuthService;

    public UpdateUserProfileCommandHandler(IUserAuthService userAuthService)
    {
        _userAuthService = userAuthService;
    }

    public async Task<UserDto> Handle(UpdateUserProfileCommand request, CancellationToken cancellationToken)
    {
        var user = await _userAuthService.FindByIdAsync(request.UserId);
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

        var (succeeded, errors) = await _userAuthService.UpdateAsync(user);
        if (!succeeded)
            throw new ValidationException(string.Join(", ", errors));

        var roles = await _userAuthService.GetRolesAsync(user);
        return GetUserProfileQueryHandler.ToDto(user, roles);
    }
}
