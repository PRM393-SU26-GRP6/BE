using CourtManager.Application.Interfaces;
using CourtManager.Domain.Entities;
using CourtManager.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CourtManager.Infrastructure.Services;

/// <summary>
/// Background service that automatically generates time slots for fields
/// when they are running low on available slots for upcoming dates.
/// </summary>
public class SlotAutoGenerateBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<SlotAutoGenerateBackgroundService> _logger;
    
    /// <summary>
    /// How many days ahead should we ensure slots exist.
    /// Default: 14 days (2 weeks ahead).
    /// </summary>
    private const int DaysAheadToGenerate = 14;
    
    /// <summary>
    /// How many days before a date should we start generating slots.
    /// Default: 1 day (tomorrow onwards).
    /// </summary>
    private const int DaysBeforeGenerateStart = 1;
    
    /// <summary>
    /// How often the service checks for slots to generate.
    /// Default: Every 1 hour.
    /// </summary>
    private static readonly TimeSpan CheckPeriod = TimeSpan.FromHours(1);

    public SlotAutoGenerateBackgroundService(IServiceProvider serviceProvider, ILogger<SlotAutoGenerateBackgroundService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("SlotAutoGenerateBackgroundService started. Will check every {Hours} hour(s).", CheckPeriod.TotalHours);

        using var timer = new PeriodicTimer(CheckPeriod);
        while (!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync(stoppingToken))
        {
            try
            {
                await GenerateMissingSlotsAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while executing SlotAutoGenerateBackgroundService");
            }
        }
    }

    private async Task GenerateMissingSlotsAsync(CancellationToken stoppingToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var fields = await context.FootballFields
            .Where(f => f.IsActive && !f.IsDeleted)
            .Include(f => f.FieldSchedules.Where(s => s.IsActive && !s.IsDeleted))
            .ToListAsync(stoppingToken);

        if (fields.Count == 0)
        {
            _logger.LogDebug("No active fields found.");
            return;
        }

        var slotsGenerated = 0;
        var slotsExistCount = 0;

        foreach (var field in fields)
        {
            // Generate slots for the next X days (starting from 7 days from now)
            for (var dayOffset = DaysBeforeGenerateStart; dayOffset <= DaysAheadToGenerate; dayOffset++)
            {
                var targetDate = today.AddDays(dayOffset);
                var dayOfWeek = (int)targetDate.DayOfWeek;

                // Get the schedule for this day of week
                var schedule = field.FieldSchedules.FirstOrDefault(s => s.DayOfWeek == dayOfWeek);
                if (schedule == null)
                {
                    _logger.LogDebug("Field {FieldId} has no schedule for {DayOfWeek}. Skipping.", field.Id, (DayOfWeek)dayOfWeek);
                    continue;
                }

                // Check how many slots exist for this field and date
                var existingSlotsCount = await context.TimeSlots
                    .CountAsync(s => s.FieldId == field.Id && s.SelectedDate == targetDate && !s.IsDeleted, stoppingToken);

                if (existingSlotsCount > 0)
                {
                    slotsExistCount++;
                    continue;
                }

                // Generate slots based on the schedule
                var slotsToAdd = GenerateSlotsForDay(field, targetDate, schedule);
                
                if (slotsToAdd.Count > 0)
                {
                    await context.TimeSlots.AddRangeAsync(slotsToAdd, stoppingToken);
                    slotsGenerated += slotsToAdd.Count;
                    _logger.LogInformation(
                        "Generated {Count} slots for Field {FieldName} ({FieldId}) on {Date}",
                        slotsToAdd.Count, field.FieldName, field.Id, targetDate);
                }
            }
        }

        if (slotsGenerated > 0)
        {
            await context.SaveChangesAsync(stoppingToken);
            _logger.LogInformation("SlotAutoGenerate: Generated {Total} new slots across {Fields} fields.", slotsGenerated, fields.Count);
        }
        else
        {
            _logger.LogDebug("SlotAutoGenerate: No new slots needed. Checked {Fields} fields.", fields.Count);
        }
    }

    private static List<TimeSlot> GenerateSlotsForDay(FootballField field, DateOnly date, FieldSchedule schedule)
    {
        var slots = new List<TimeSlot>();
        var currentTime = schedule.OpenTime;
        var slotIndex = 1;

        while (currentTime.AddMinutes(schedule.SlotDurationMinutes) <= schedule.CloseTime)
        {
            var endTime = currentTime.AddMinutes(schedule.SlotDurationMinutes);

            slots.Add(new TimeSlot
            {
                SlotId = Guid.NewGuid(),
                FieldId = field.Id,
                StartTime = currentTime,
                EndTime = endTime,
                SelectedDate = date,
                Price = field.PricePerHour,
                SlotStatus = SlotStatus.Available,
                CreatedAt = DateTime.UtcNow,
                RowVersion = (uint)(date.DayNumber * 1000 + slotIndex) // Deterministic RowVersion based on date
            });

            currentTime = endTime;
            slotIndex++;
        }

        return slots;
    }
}
