using CourtManager.Application.DTOs;
using MediatR;

namespace CourtManager.Application.Features.Auth.Commands;

/// <summary>
/// Updates the current user's profile (full name, phone, avatar URL).
/// UserId is resolved from ICurrentUserService, not from the request.
/// </summary>
public class UpdateProfileCommand : IRequest<UserDto>
{
    public string FullName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? AvatarUrl { get; set; }
}
