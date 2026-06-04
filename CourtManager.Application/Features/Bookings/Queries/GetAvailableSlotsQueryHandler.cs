using AutoMapper;
using CourtManager.Application.DTOs;
using CourtManager.Application.Interfaces;
using MediatR;

namespace CourtManager.Application.Features.Bookings.Queries;

/// <summary>
/// Handler for GetAvailableSlotsQuery.
/// Retrieves available time slots for a specific field and date.
/// </summary>
public class GetAvailableSlotsQueryHandler : IRequestHandler<GetAvailableSlotsQuery, IEnumerable<TimeSlotDto>>
{
    private readonly ITimeSlotRepository _timeSlotRepository;
    private readonly IMapper _mapper;

    public GetAvailableSlotsQueryHandler(ITimeSlotRepository timeSlotRepository, IMapper mapper)
    {
        _timeSlotRepository = timeSlotRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<TimeSlotDto>> Handle(GetAvailableSlotsQuery request, CancellationToken cancellationToken)
    {
        var slots = await _timeSlotRepository.GetAvailableSlotsAsync(request.FieldId, request.Date, cancellationToken);
        return _mapper.Map<IEnumerable<TimeSlotDto>>(slots);
    }
}
