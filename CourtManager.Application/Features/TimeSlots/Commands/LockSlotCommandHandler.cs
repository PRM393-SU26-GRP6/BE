using CourtManager.Application.Exceptions;
using CourtManager.Domain.Entities;
using CourtManager.Domain.Enums;
using CourtManager.Application.Interfaces;
using MediatR;

namespace CourtManager.Application.Features.TimeSlots.Commands;

public class LockSlotCommandHandler : IRequestHandler<LockSlotCommand, LockSlotResult>
{
    private readonly ITimeSlotRepository _timeSlotRepository;
    private readonly IFootballFieldRepository _fieldRepository;

    public LockSlotCommandHandler(ITimeSlotRepository timeSlotRepository, IFootballFieldRepository fieldRepository)
    {
        _timeSlotRepository = timeSlotRepository;
        _fieldRepository = fieldRepository;
    }

    public async Task<LockSlotResult> Handle(LockSlotCommand request, CancellationToken cancellationToken)
    {
        // Validate not in past
        var now = DateTime.UtcNow;
        if (request.SelectedDate < DateOnly.FromDateTime(now) ||
            (request.SelectedDate == DateOnly.FromDateTime(now) && request.StartTime < TimeOnly.FromDateTime(now)))
        {
            throw new InvalidOperationException("Cannot lock a slot that has already started or is in the past.");
        }

        // Find or create the slot
        var slot = await FindOrCreateSlotAsync(request, cancellationToken);

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

        // Reload to get updated lock info (bypass EF cache)
        slot = await _timeSlotRepository.GetByIdAsNoTrackingAsync(slot.SlotId, cancellationToken);

        return new LockSlotResult
        {
            SlotId = slot!.SlotId,
            LockedUntil = slot.LockedUntil!.Value,
            IsNewSlot = false
        };
    }

    private async Task<TimeSlot> FindOrCreateSlotAsync(LockSlotCommand request, CancellationToken cancellationToken)
    {
        if (request.SlotId.HasValue)
        {
            var existing = await _timeSlotRepository.GetByIdAsync(request.SlotId.Value, cancellationToken);
            if (existing == null || existing.IsDeleted)
            {
                throw new NotFoundException("Slot", request.SlotId.Value);
            }
            return existing;
        }

        // Find existing slot by field/date/time (in case it was created by another user)
        var existingSlot = await _timeSlotRepository.GetByFieldDateTimeAsync(
            request.FieldId,
            request.SelectedDate,
            request.StartTime,
            request.EndTime,
            cancellationToken);

        if (existingSlot != null && !existingSlot.IsDeleted)
        {
            return existingSlot;
        }

        // Create new slot
        var field = await _fieldRepository.GetByIdAsync(request.FieldId, cancellationToken);
        if (field == null)
        {
            throw new NotFoundException("Field", request.FieldId);
        }

        var newSlot = new TimeSlot
        {
            SlotId = Guid.NewGuid(),
            FieldId = request.FieldId,
            StartTime = request.StartTime,
            EndTime = request.EndTime,
            SelectedDate = request.SelectedDate,
            Price = field.PricePerHour,
            SlotStatus = SlotStatus.Available,
            CreatedAt = DateTime.UtcNow
        };

        await _timeSlotRepository.AddAsync(newSlot, cancellationToken);
        await _timeSlotRepository.SaveChangesAsync(cancellationToken);

        return newSlot;
    }
}
