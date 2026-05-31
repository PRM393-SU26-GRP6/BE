using CourtManager.Application.DTOs;
using MediatR;

namespace CourtManager.Application.Features.TimeSlots.Queries;

public record GetAvailableSlotsQuery(
    Guid FieldId,
    DateTime Date
) : IRequest<IEnumerable<TimeSlotDto>>;
