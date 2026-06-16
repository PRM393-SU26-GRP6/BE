using CourtManager.Application.Exceptions;
using CourtManager.Domain.Enums;
using CourtManager.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CourtManager.Application.Features.TimeSlots.Commands;

public class UnlockSlotCommandHandler : IRequestHandler<UnlockSlotCommand, bool>
{
    private readonly ITimeSlotRepository _timeSlotRepository;
    private readonly IBookingRepository _bookingRepository;

    public UnlockSlotCommandHandler(ITimeSlotRepository timeSlotRepository, IBookingRepository bookingRepository)
    {
        _timeSlotRepository = timeSlotRepository;
        _bookingRepository = bookingRepository;
    }

    public async Task<bool> Handle(UnlockSlotCommand request, CancellationToken cancellationToken)
    {
        var slot = await _timeSlotRepository.GetByIdWithFieldVenueAsync(request.SlotId, cancellationToken);
        if (slot == null || slot.IsDeleted)
        {
            throw new NotFoundException("Slot", request.SlotId);
        }

        if (slot.SlotStatus != SlotStatus.Locked)
        {
            throw new InvalidOperationException("Only locked slots can be unlocked.");
        }

        var activeBooking = slot.BookingItems
            .Select(bi => bi.Booking)
            .FirstOrDefault(b => b != null && !b.IsDeleted);

        var isBookingOwner = activeBooking?.UserId == request.UserId;
        var isSlotLocker = slot.LockedBy == request.UserId;

        if (!isBookingOwner && !isSlotLocker)
        {
            throw new ValidationException("You are not allowed to unlock this slot.");
        }

        // Soft delete the slot - unlock means the slot becomes available again
        // but we mark it as deleted so it won't show up in queries
        // The slot will be regenerated when another user locks it
        slot.IsDeleted = true;
        slot.DeletedAt = DateTime.UtcNow;
        slot.UpdatedAt = DateTime.UtcNow;

        try
        {
            await _timeSlotRepository.UpdateAsync(slot, cancellationToken);
            await _timeSlotRepository.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new ValidationException("This slot was updated by another request. Please refresh and try again.");
        }

        return true;
    }
}
