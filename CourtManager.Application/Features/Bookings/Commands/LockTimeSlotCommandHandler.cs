using CourtManager.Application.Exceptions;
using CourtManager.Domain.Entities;
using CourtManager.Domain.Interfaces;
using MediatR;

namespace CourtManager.Application.Features.Bookings.Commands;

/// <summary>
/// Handler for LockTimeSlotCommand.
/// Implements the business logic for locking a time slot during checkout.
/// Lock prevents other bookings from using the slot while payment is processing.
/// </summary>
public class LockTimeSlotCommandHandler : IRequestHandler<LockTimeSlotCommand, bool>
{
    private readonly ITimeSlotRepository _timeSlotRepository;

    public LockTimeSlotCommandHandler(ITimeSlotRepository timeSlotRepository)
    {
        _timeSlotRepository = timeSlotRepository;
    }

    /// <summary>
    /// Handles the LockTimeSlotCommand.
    /// Verifies slot exists and is available, then locks it.
    /// </summary>
    public async Task<bool> Handle(LockTimeSlotCommand request, CancellationToken cancellationToken)
    {
        // Fetch the time slot
        var slot = await _timeSlotRepository.GetByIdAsync(request.SlotId, cancellationToken);
        if (slot == null)
            throw new NotFoundException(nameof(TimeSlot), request.SlotId);

        // Verify slot is available
        if (slot.SlotStatus != CourtManager.Domain.Enums.SlotStatus.Available)
            throw new ValidationException(
                $"Cannot lock slot. Current status is '{slot.SlotStatus}'. Only 'Available' slots can be locked.");

        // Lock the slot
        slot.SlotStatus = CourtManager.Domain.Enums.SlotStatus.Locked;
        slot.LockedUntil = DateTime.UtcNow.AddMinutes(15);
        slot.LockedBy = request.UserId;
        slot.UpdatedAt = DateTime.UtcNow;

        // Save changes
        //await _timeSlotRepository.UpdateSlotStatusAsync(request.SlotId, "Locked", cancellationToken);
        await _timeSlotRepository.SaveChangesAsync(cancellationToken);

        return true;
    }
}
