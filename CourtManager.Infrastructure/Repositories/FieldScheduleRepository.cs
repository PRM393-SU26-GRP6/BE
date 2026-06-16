using CourtManager.Application.Interfaces;
using CourtManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CourtManager.Infrastructure.Repositories;

public class FieldScheduleRepository : IFieldScheduleRepository
{
    private readonly ApplicationDbContext _context;

    public FieldScheduleRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<FieldSchedule>> GetByFieldIdAsync(
        Guid fieldId,
        CancellationToken cancellationToken = default)
    {
        return await _context.FieldSchedules
            .Where(s => s.FieldId == fieldId)
            .OrderBy(s => s.DayOfWeek)
            .ToListAsync(cancellationToken);
    }

    public async Task<FieldSchedule?> GetForDayAsync(
        Guid fieldId,
        int dayOfWeek,
        CancellationToken cancellationToken = default)
    {
        return await _context.FieldSchedules
            .FirstOrDefaultAsync(
                s => s.FieldId == fieldId && s.DayOfWeek == dayOfWeek,
                cancellationToken);
    }

    public async Task ReplaceAllForFieldAsync(
        Guid fieldId,
        IEnumerable<FieldSchedule> schedules,
        CancellationToken cancellationToken = default)
    {
        var incoming = schedules.ToList();

        await using var tx = await _context.Database.BeginTransactionAsync(cancellationToken);

        var existing = await _context.FieldSchedules
            .Where(s => s.FieldId == fieldId)
            .ToListAsync(cancellationToken);

        var incomingByDay = incoming.ToDictionary(s => s.DayOfWeek);

        // Delete rows that are no longer in the incoming list
        var toDelete = existing.Where(e => !incomingByDay.ContainsKey(e.DayOfWeek)).ToList();
        if (toDelete.Any())
        {
            _context.FieldSchedules.RemoveRange(toDelete);
        }

        // Update or insert
        foreach (var inc in incoming)
        {
            var ex = existing.FirstOrDefault(e => e.DayOfWeek == inc.DayOfWeek);
            if (ex != null)
            {
                ex.OpenTime = inc.OpenTime;
                ex.CloseTime = inc.CloseTime;
                ex.SlotDurationMinutes = inc.SlotDurationMinutes;
                ex.IsActive = inc.IsActive;
                ex.UpdatedAt = inc.UpdatedAt;
            }
            else
            {
                _context.FieldSchedules.Add(inc);
            }
        }

        await _context.SaveChangesAsync(cancellationToken);
        await tx.CommitAsync(cancellationToken);
    }
}
