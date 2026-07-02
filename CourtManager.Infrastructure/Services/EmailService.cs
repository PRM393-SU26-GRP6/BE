using System.Net;
using CourtManager.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;

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

        var email = new MimeMessage();
        email.From.Add(new MailboxAddress("CourtManager", senderEmail));
        email.To.Add(MailboxAddress.Parse(to));
        email.Subject = subject;
        email.Body = new TextPart(TextFormat.Html) { Text = body };

        using var smtp = new SmtpClient();
        smtp.Timeout = 5000; // 5 seconds timeout
        
        try
        {
            await smtp.ConnectAsync(host, port, SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(senderEmail, senderPassword);
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"MailKit failed to send email: {ex.Message}");
        }
    }
}
