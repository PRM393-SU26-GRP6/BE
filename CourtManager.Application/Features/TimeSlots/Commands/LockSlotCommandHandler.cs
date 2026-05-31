using CourtManager.Application.Exceptions;
using CourtManager.Domain.Enums;
using CourtManager.Domain.Interfaces;
using MediatR;

namespace CourtManager.Application.Features.TimeSlots.Commands;

public class LockSlotCommandHandler : IRequestHandler<LockSlotCommand, bool>
{
    private readonly ITimeSlotRepository _timeSlotRepository;

    public LockSlotCommandHandler(ITimeSlotRepository timeSlotRepository)
    {
        _timeSlotRepository = timeSlotRepository;
    }

    public async Task<bool> Handle(LockSlotCommand request, CancellationToken cancellationToken)
    {
        var slot = await _timeSlotRepository.GetByIdAsync(request.SlotId, cancellationToken);
        if (slot == null || slot.IsDeleted)
        {
            throw new NotFoundException("Slot", request.SlotId);
        }

        if (slot.StartTime <= DateTime.UtcNow)
        {
            throw new InvalidOperationException("Cannot lock a slot that has already started or is in the past.");
        }

        // Check if slot is available or its lock has expired
        bool isAvailable = slot.SlotStatus == SlotStatus.Available ||
                           (slot.SlotStatus == SlotStatus.Locked && slot.LockedUntil.HasValue && slot.LockedUntil.Value < DateTime.UtcNow);

        if (!isAvailable)
        {
            throw new InvalidOperationException("This slot is no longer available.");
        }

        // Lock the slot for 15 minutes
        slot.SlotStatus = SlotStatus.Locked;
        slot.LockedBy = request.UserId;
        slot.LockedUntil = DateTime.UtcNow.AddMinutes(15);
        slot.UpdatedAt = DateTime.UtcNow;

        await _timeSlotRepository.UpdateAsync(slot, cancellationToken);
        await _timeSlotRepository.SaveChangesAsync(cancellationToken);

        return true;
    }
}
