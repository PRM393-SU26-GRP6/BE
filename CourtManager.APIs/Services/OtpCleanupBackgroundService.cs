using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using CourtManager.Domain.Entities;

namespace CourtManager.APIs.Services;

public class OtpCleanupBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<OtpCleanupBackgroundService> _logger;

    public OtpCleanupBackgroundService(
        IServiceProvider serviceProvider,
        ILogger<OtpCleanupBackgroundService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await CleanupExpiredOtpsAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred executing OTP cleanup job.");
            }

            // Run every 5 minutes
            await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
        }
    }

    private async Task CleanupExpiredOtpsAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        
        var now = DateTime.UtcNow;
        var expiredUsers = await userManager.Users
            .Where(u => u.OtpCode != null && u.OtpExpiryTime != null && u.OtpExpiryTime < now)
            .ToListAsync(cancellationToken);

        if (expiredUsers.Any())
        {
            foreach (var user in expiredUsers)
            {
                user.OtpCode = null;
                user.OtpExpiryTime = null;
                user.OtpAttempts = 0;
                await userManager.UpdateAsync(user);
            }

            _logger.LogInformation("Cleaned up {Count} expired OTPs.", expiredUsers.Count);
        }
    }
}
