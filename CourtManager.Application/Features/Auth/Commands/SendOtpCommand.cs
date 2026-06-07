using MediatR;
using CourtManager.Application.DTOs;

namespace CourtManager.Application.Features.Auth.Commands;

public class SendOtpCommand : IRequest<SendOtpResponseDto>
{
    public string Email { get; set; } = string.Empty;
}

public class SendOtpResponseDto
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
}
