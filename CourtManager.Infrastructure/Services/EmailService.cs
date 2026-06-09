using System.Net;
using System.Net.Mail;
using CourtManager.Application.Interfaces;
using Microsoft.Extensions.Configuration;

namespace CourtManager.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;

    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendEmailAsync(string to, string subject, string body)
    {
        var host = _configuration["EmailSettings:Host"] ?? "smtp.gmail.com";
        var portStr = _configuration["EmailSettings:Port"] ?? "587";
        int.TryParse(portStr, out var port);
        if (port == 0) port = 587;
        
        var senderEmail = _configuration["EmailSettings:SenderEmail"] ?? string.Empty;
        var senderPassword = _configuration["EmailSettings:SenderPassword"] ?? string.Empty;

        using var client = new SmtpClient(host, port)
        {
            Credentials = new NetworkCredential(senderEmail, senderPassword),
            EnableSsl = true
        };

        var mailMessage = new MailMessage
        {
            From = new MailAddress(senderEmail, "CourtManager"),
            Subject = subject,
            Body = body,
            IsBodyHtml = true
        };

        mailMessage.To.Add(to);

        await client.SendMailAsync(mailMessage);
    }
}
