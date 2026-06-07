using CourtManager.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Mail;

namespace CourtManager.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<bool> SendOtpAsync(string email, string otp, CancellationToken cancellationToken = default)
    {
        var subject = "Xác minh email tài khoản của bạn";
        var body = $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <style>
        body {{ font-family: Arial, sans-serif; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background-color: #4CAF50; color: white; padding: 20px; text-align: center; border-radius: 5px; }}
        .content {{ padding: 20px; background-color: #f9f9f9; margin-top: 20px; border-radius: 5px; }}
        .otp-code {{ font-size: 28px; font-weight: bold; color: #4CAF50; text-align: center; padding: 20px; background-color: #f0f0f0; border-radius: 5px; margin: 20px 0; }}
        .footer {{ margin-top: 20px; font-size: 12px; color: #999; text-align: center; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h2>CourtManager - Xác minh Email</h2>
        </div>
        <div class='content'>
            <p>Xin chào,</p>
            <p>Bạn đã yêu cầu xác minh email cho tài khoản của mình. Vui lòng sử dụng mã OTP dưới đây:</p>
            <div class='otp-code'>{otp}</div>
            <p>Mã OTP này có hiệu lực trong <strong>10 phút</strong>.</p>
            <p>Nếu bạn không thực hiện yêu cầu này, vui lòng bỏ qua email này.</p>
        </div>
        <div class='footer'>
            <p>&copy; 2026 CourtManager. Tất cả các quyền được bảo lưu.</p>
        </div>
    </div>
</body>
</html>";

        return await SendEmailAsync(email, subject, body, cancellationToken);
    }

    public async Task<bool> SendEmailAsync(string to, string subject, string body, CancellationToken cancellationToken = default)
    {
        try
        {
            var emailSettings = _configuration.GetSection("EmailSettings");
            var host = emailSettings["Host"];
            var port = int.Parse(emailSettings["Port"] ?? "587");
            var senderEmail = emailSettings["SenderEmail"];
            var senderPassword = emailSettings["SenderPassword"];

            using (var client = new SmtpClient(host, port))
            {
                client.EnableSsl = true;
                client.Credentials = new NetworkCredential(senderEmail, senderPassword);
                client.Timeout = 10000;

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(senderEmail!),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };

                mailMessage.To.Add(to);

                await client.SendMailAsync(mailMessage);
                _logger.LogInformation($"Email sent successfully to {to}");
                return true;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error sending email to {to}: {ex.Message}");
            return false;
        }
    }
}
