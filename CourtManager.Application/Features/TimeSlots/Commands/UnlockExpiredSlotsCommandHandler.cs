using CourtManager.Application.Interfaces;
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

        var expiredSlots = (await _timeSlotRepository.GetLockedSlotsExpiredAsync(cancellationToken)).ToList();

        if (expiredSlots.Count == 0)
        {
            return 0;
        }

        foreach (var slot in expiredSlots)
        {
            // Soft delete - slot will be regenerated when another user locks it
            slot.IsDeleted = true;
            slot.DeletedAt = DateTime.UtcNow;
            await _timeSlotRepository.UpdateAsync(slot, cancellationToken);
        }

        await _timeSlotRepository.SaveChangesAsync(cancellationToken);

        var count = expiredSlots.Count;
        _logger.LogInformation("Soft-deleted {Count} expired slots.", count);
        return count;
    }
}
