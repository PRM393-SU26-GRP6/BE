using MediatR;

namespace CourtManager.Application.Features.TimeSlots.Commands;

public record LockSlotCommand(Guid SlotId, Guid UserId) : IRequest<bool>;
