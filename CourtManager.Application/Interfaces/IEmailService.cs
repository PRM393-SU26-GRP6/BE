namespace CourtManager.Application.Interfaces;

public interface IEmailService
{
    Task<bool> SendOtpAsync(string email, string otp, CancellationToken cancellationToken = default);
    Task<bool> SendEmailAsync(string to, string subject, string body, CancellationToken cancellationToken = default);
}
