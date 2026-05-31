using CourtManager.Application.Exceptions;
using CourtManager.Domain.Enums;
using CourtManager.Domain.Interfaces;
using MediatR;

namespace CourtManager.Application.Features.TimeSlots.Commands;

public class UnlockSlotCommandHandler : IRequestHandler<UnlockSlotCommand, bool>
{
    private readonly ITimeSlotRepository _timeSlotRepository;

    public UnlockSlotCommandHandler(ITimeSlotRepository timeSlotRepository)
    {
        _timeSlotRepository = timeSlotRepository;
    }

    public async Task<bool> Handle(UnlockSlotCommand request, CancellationToken cancellationToken)
    {
        var slot = await _timeSlotRepository.GetByIdAsync(request.SlotId, cancellationToken);
        if (slot == null || slot.IsDeleted)
        {
            throw new NotFoundException("Slot", request.SlotId);
        }

        // Check if the slot is currently locked by the user
        if (slot.SlotStatus != SlotStatus.Locked || slot.LockedBy != request.UserId)
        {
            throw new InvalidOperationException("You can only unlock slots that you have locked.");
        }

        // Unlock the slot
        slot.SlotStatus = SlotStatus.Available;
        slot.LockedBy = null;
        slot.LockedUntil = null;
        slot.UpdatedAt = DateTime.UtcNow;

        await _timeSlotRepository.UpdateAsync(slot, cancellationToken);
        await _timeSlotRepository.SaveChangesAsync(cancellationToken);

        return true;
    }
}
