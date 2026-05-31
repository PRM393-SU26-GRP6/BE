using MediatR;

namespace CourtManager.Application.Features.TimeSlots.Commands;

public record UnlockSlotCommand(Guid SlotId, Guid UserId) : IRequest<bool>;
