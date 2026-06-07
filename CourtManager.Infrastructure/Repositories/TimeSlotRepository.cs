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

    public async Task<TimeSlot?> GetByIdWithFieldVenueAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(s => s.Field)
                .ThenInclude(f => f!.Venue)
            .Include(s => s.LockedByUser)
            .Include(s => s.BookingItems)
                .ThenInclude(bi => bi.Booking)
            .FirstOrDefaultAsync(s => s.SlotId == id, cancellationToken);
    }

    public async Task<bool> TryLockSlotAtomicAsync(Guid slotId, Guid userId, CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        var lockExpiry = now.AddMinutes(15);

        var slotVersion = await _dbSet
            .AsNoTracking()
            .Where(s => s.SlotId == slotId)
            .Select(s => s.RowVersion)
            .FirstOrDefaultAsync(cancellationToken);

        if (slotVersion == null)
            return false;

        // Atomic update: only succeeds if the slot is still available and its row version has not changed.
        // Note: SlotStatus stored as varchar in DB, use string values for comparison
        var rowsAffected = await _context.Database.ExecuteSqlInterpolatedAsync(
            $@"
                UPDATE ""TimeSlots""
                SET ""SlotStatus"" = 'Locked',
                    ""LockedBy"" = {userId},
                    ""LockedUntil"" = {lockExpiry},
                    ""UpdatedAt"" = {now}
                WHERE ""SlotId"" = {slotId}
                  AND ""SlotStatus"" = 'Available'
                  AND (""LockedUntil"" IS NULL OR ""LockedUntil"" < {now})
                  AND ""RowVersion"" = {slotVersion}",
            cancellationToken);

        return rowsAffected > 0;
    }
}
