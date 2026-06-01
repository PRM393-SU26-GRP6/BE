using MediatR;

namespace CourtManager.Application.Features.TimeSlots.Commands;

public class UnlockExpiredSlotsCommand : IRequest<int>
{
}
