using MediatR;
using CourtManager.Application.DTOs;

namespace CourtManager.Application.Features.Auth.Commands;

public class ResendOtpCommand : IRequest<AuthResponseDto>
{
    public string Email { get; set; } = string.Empty;
}
