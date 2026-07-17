using CourtManager.Application.Exceptions;
using CourtManager.Domain.Enums;
using CourtManager.Application.Interfaces;
using MediatR;

namespace CourtManager.Application.Features.TimeSlots.Commands;

public class LockSlotCommandHandler : IRequestHandler<LockSlotCommand, LockSlotResult>
{
    private readonly ITimeSlotRepository _timeSlotRepository;

    public LockSlotCommandHandler(ITimeSlotRepository timeSlotRepository)
    {
        _timeSlotRepository = timeSlotRepository;
    }

    public async Task<LockSlotResult> Handle(LockSlotCommand request, CancellationToken cancellationToken)
    {
        if (request.SlotId == Guid.Empty)
        {
            throw new ValidationException("SlotId is required.");
        }

        var slot = await _timeSlotRepository.GetByIdAsync(request.SlotId, cancellationToken);
        if (slot == null || slot.IsDeleted)
        {
            throw new NotFoundException("Slot", request.SlotId);
        }

        var now = DateTime.UtcNow;
        if (slot.SelectedDate < DateOnly.FromDateTime(now) ||
            (slot.SelectedDate == DateOnly.FromDateTime(now) && slot.StartTime < TimeOnly.FromDateTime(now)))
        {
            throw new InvalidOperationException("Cannot lock a slot that has already started or is in the past.");
        }

        if (slot.SlotStatus == SlotStatus.Booked)
        {
            throw new InvalidOperationException("This slot is already booked.");
        }

        // If already locked by current user and lock is still valid, return success
        if (slot.SlotStatus == SlotStatus.Locked &&
            slot.LockedBy == request.UserId &&
            slot.LockedUntil.HasValue &&
            slot.LockedUntil.Value > now)
        {
            return new LockSlotResult
            {
                SlotId = slot.SlotId,
                LockedUntil = slot.LockedUntil.Value,
                IsNewSlot = false
            };
        }

        // Try to acquire lock atomically
        var lockAcquired = await _timeSlotRepository.TryLockSlotAtomicAsync(slot.SlotId, request.UserId, cancellationToken);
        if (!lockAcquired)
        {
            throw new InvalidOperationException("This slot is no longer available.");
        }

        slot = await _timeSlotRepository.GetByIdAsNoTrackingAsync(slot.SlotId, cancellationToken);
        if (slot?.LockedUntil == null)
        {
            throw new InvalidOperationException("The slot lock could not be reloaded.");
        }

        return new LockSlotResult
        {
            SlotId = slot.SlotId,
            LockedUntil = slot.LockedUntil.Value,
            IsNewSlot = false
        };
    }
}
