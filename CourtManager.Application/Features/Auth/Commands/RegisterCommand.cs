using MediatR;
using CourtManager.Application.DTOs;

namespace CourtManager.Application.Features.Auth.Commands;

/// <summary>
/// Command for user registration.
/// </summary>
public class RegisterCommand : IRequest<AuthResponseDto>
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
}
