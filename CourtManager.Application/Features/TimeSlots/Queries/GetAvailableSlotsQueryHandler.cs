using CourtManager.Application.DTOs;
using CourtManager.Domain.Interfaces;
using MediatR;

namespace CourtManager.Application.Features.TimeSlots.Queries;

public class GetAvailableSlotsQueryHandler : IRequestHandler<GetAvailableSlotsQuery, IEnumerable<TimeSlotDto>>
{
    private readonly ITimeSlotRepository _timeSlotRepository;

    public GetAvailableSlotsQueryHandler(ITimeSlotRepository timeSlotRepository)
    {
        _timeSlotRepository = timeSlotRepository;
    }

    public async Task<IEnumerable<TimeSlotDto>> Handle(GetAvailableSlotsQuery request, CancellationToken cancellationToken)
    {
        var slots = await _timeSlotRepository.GetAvailableSlotsAsync(request.FieldId, request.Date, cancellationToken);
        
        return slots.Select(s => new TimeSlotDto
        {
            SlotId = s.SlotId,
            FieldId = s.FieldId,
            StartTime = s.StartTime,
            EndTime = s.EndTime,
            SlotStatus = s.SlotStatus.ToString(),
            CreatedAt = s.CreatedAt,
            UpdatedAt = s.UpdatedAt
        });
    }
}
