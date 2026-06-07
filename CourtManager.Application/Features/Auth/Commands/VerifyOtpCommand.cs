using MediatR;
using CourtManager.Application.DTOs;

namespace CourtManager.Application.Features.Auth.Commands;

public class VerifyOtpCommand : IRequest<AuthResponseDto>
{
    public string Email { get; set; } = string.Empty;
    public string Otp { get; set; } = string.Empty;
}
