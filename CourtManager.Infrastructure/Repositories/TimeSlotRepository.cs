using Microsoft.EntityFrameworkCore;
using CourtManager.Domain.Entities;
using CourtManager.Domain.Interfaces;

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
        throw new NotImplementedException();
    }

    public async Task UpdateSlotStatusAsync(Guid slotId, string status, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<TimeSlot>> GetLockedSlotsAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<TimeSlot>> GetLockedSlotsExpiredAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task BatchUpdateSlotStatusAsync(IEnumerable<Guid> slotIds, string status, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
