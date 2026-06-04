using Microsoft.EntityFrameworkCore;
using CourtManager.Domain.Entities;
using CourtManager.Application.Interfaces;

namespace CourtManager.Infrastructure.Repositories;

public class TimeSlotRepository : Repository<TimeSlot>, ITimeSlotRepository
{
    public TimeSlotRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IEnumerable<TimeSlot>> GetAvailableSlotsAsync(Guid fieldId, DateTime date, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(x => x.FieldId == fieldId && !x.IsDeleted && x.StartTime.Date == date.Date)
            .OrderBy(x => x.StartTime)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<TimeSlot>> GetSlotsByFieldIdAsync(Guid fieldId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(x => x.FieldId == fieldId && !x.IsDeleted)
            .OrderBy(x => x.StartTime)
            .ToListAsync(cancellationToken);
    }

    public async Task UpdateSlotStatusAsync(Guid slotId, string status, CancellationToken cancellationToken = default)
    {
        var slot = await GetByIdAsync(slotId, cancellationToken);
        if (slot != null && Enum.TryParse<CourtManager.Domain.Enums.SlotStatus>(status, true, out var parsedStatus))
        {
            slot.SlotStatus = parsedStatus;
            await UpdateAsync(slot, cancellationToken);
        }
    }

    public async Task<IEnumerable<TimeSlot>> GetLockedSlotsAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(x => x.SlotStatus == CourtManager.Domain.Enums.SlotStatus.Locked && !x.IsDeleted)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<TimeSlot>> GetLockedSlotsExpiredAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        return await _dbSet
            .Where(x => x.SlotStatus == CourtManager.Domain.Enums.SlotStatus.Locked && x.LockedUntil < now && !x.IsDeleted)
            .ToListAsync(cancellationToken);
    }

    public async Task BatchUpdateSlotStatusAsync(IEnumerable<Guid> slotIds, string status, CancellationToken cancellationToken = default)
    {
        if (Enum.TryParse<CourtManager.Domain.Enums.SlotStatus>(status, true, out var parsedStatus))
        {
            var slots = await _dbSet.Where(x => slotIds.Contains(x.SlotId)).ToListAsync(cancellationToken);
            foreach (var slot in slots)
            {
                slot.SlotStatus = parsedStatus;
            }
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
