using CourtManager.Application.Features.TimeSlots.Commands;
using MediatR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace CourtManager.APIs.Services;

public class SlotUnlockBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<SlotUnlockBackgroundService> _logger;
    private readonly TimeSpan _period = TimeSpan.FromMinutes(1);

    public SlotUnlockBackgroundService(IServiceProvider serviceProvider, ILogger<SlotUnlockBackgroundService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(_period);
        while (!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync(stoppingToken))
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

                var unlockedCount = await mediator.Send(new UnlockExpiredSlotsCommand(), stoppingToken);
                
                if (unlockedCount > 0)
                {
                    _logger.LogInformation("BackgroundService: Unlocked {Count} expired slots.", unlockedCount);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while executing SlotUnlockBackgroundService");
            }
        }
    }
}
