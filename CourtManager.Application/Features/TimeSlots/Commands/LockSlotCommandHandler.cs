using CourtManager.Application.Exceptions;
using CourtManager.Domain.Enums;
using CourtManager.Application.Interfaces;
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

        if (slot.SelectedDate < DateOnly.FromDateTime(DateTime.UtcNow) ||
            (slot.SelectedDate == DateOnly.FromDateTime(DateTime.UtcNow) && slot.StartTime < TimeOnly.FromDateTime(DateTime.UtcNow)))
        {
            throw new InvalidOperationException("Cannot lock a slot that has already started or is in the past.");
        }

        if (slot.SlotStatus == SlotStatus.Booked)
        {
            throw new InvalidOperationException("This slot is no longer available.");
        }

        if (slot.SlotStatus == SlotStatus.Locked &&
            slot.LockedBy == request.UserId &&
            slot.LockedUntil.HasValue &&
            slot.LockedUntil.Value > DateTime.UtcNow)
        {
            return true;
        }

        var lockAcquired = await _timeSlotRepository.TryLockSlotAtomicAsync(request.SlotId, request.UserId, cancellationToken);
        if (!lockAcquired)
        {
            throw new InvalidOperationException("This slot is no longer available.");
        }

        return true;
    }
}
