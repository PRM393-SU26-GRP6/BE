using CourtManager.Application.Exceptions;
using CourtManager.Domain.Entities;
using CourtManager.Application.Interfaces;
using MediatR;

namespace CourtManager.Application.Features.Bookings.Commands;

/// <summary>
/// Handler for UnlockTimeSlotCommand.
/// Implements the business logic for unlocking a time slot.
/// Reverts locked slots back to available when payment fails, times out, or is refunded.
/// </summary>
public class UnlockTimeSlotCommandHandler : IRequestHandler<UnlockTimeSlotCommand, bool>
{
    private readonly ITimeSlotRepository _timeSlotRepository;

    public UnlockTimeSlotCommandHandler(ITimeSlotRepository timeSlotRepository)
    {
        _timeSlotRepository = timeSlotRepository;
    }

    /// <summary>
    /// Handles the UnlockTimeSlotCommand.
    /// Verifies slot exists and is locked, then unlocks it.
    /// </summary>
    public async Task<bool> Handle(UnlockTimeSlotCommand request, CancellationToken cancellationToken)
    {
        // Fetch the time slot
        var slot = await _timeSlotRepository.GetByIdAsync(request.SlotId, cancellationToken);
        if (slot == null)
            throw new NotFoundException(nameof(TimeSlot), request.SlotId);

        // Verify slot is locked
        if (slot.SlotStatus != CourtManager.Domain.Enums.SlotStatus.Locked)
            throw new ValidationException(
                $"Cannot unlock slot. Current status is '{slot.SlotStatus}'. Only 'Locked' slots can be unlocked.");

        // Unlock the slot
        slot.SlotStatus = CourtManager.Domain.Enums.SlotStatus.Available;
        slot.UpdatedAt = DateTime.UtcNow;

        // Save changes
        //await _timeSlotRepository.UpdateSlotStatusAsync(request.SlotId, "Available", cancellationToken);
        await _timeSlotRepository.SaveChangesAsync(cancellationToken);

        return true;
    }
}
