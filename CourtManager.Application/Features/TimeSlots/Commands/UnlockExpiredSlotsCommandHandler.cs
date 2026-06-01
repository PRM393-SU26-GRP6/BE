using CourtManager.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CourtManager.Application.Features.TimeSlots.Commands;

public class UnlockExpiredSlotsCommandHandler : IRequestHandler<UnlockExpiredSlotsCommand, int>
{
    private readonly ITimeSlotRepository _timeSlotRepository;
    private readonly ILogger<UnlockExpiredSlotsCommandHandler> _logger;

    public UnlockExpiredSlotsCommandHandler(ITimeSlotRepository timeSlotRepository, ILogger<UnlockExpiredSlotsCommandHandler> logger)
    {
        _timeSlotRepository = timeSlotRepository;
        _logger = logger;
    }

    public async Task<int> Handle(UnlockExpiredSlotsCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Executing background task to unlock expired slots.");
        
        var allSlots = await _timeSlotRepository.GetAllAsync(cancellationToken);
        
        var expiredSlots = allSlots.Where(s => 
            s.SlotStatus == CourtManager.Domain.Enums.SlotStatus.Locked && 
            s.LockedUntil.HasValue && 
            s.LockedUntil.Value < DateTime.UtcNow).ToList();

        if (!expiredSlots.Any())
        {
            return 0;
        }

        foreach (var slot in expiredSlots)
        {
            slot.SlotStatus = CourtManager.Domain.Enums.SlotStatus.Available;
            slot.LockedUntil = null;
            slot.LockedBy = null;
            await _timeSlotRepository.UpdateAsync(slot, cancellationToken);
        }

        await _timeSlotRepository.SaveChangesAsync(cancellationToken);
        
        _logger.LogInformation("Unlocked {Count} expired slots.", expiredSlots.Count);
        return expiredSlots.Count;
    }
}
