using AutoMapper;
using CourtManager.Application.DTOs;
using CourtManager.Application.Exceptions;
using CourtManager.Domain.Entities;
using CourtManager.Application.Interfaces;
using MediatR;

namespace CourtManager.Application.Features.Bookings.Queries;

/// <summary>
/// Handler for GetTimeSlotQuery.
/// Retrieves a specific time slot by ID.
/// </summary>
public class GetTimeSlotQueryHandler : IRequestHandler<GetTimeSlotQuery, TimeSlotDto>
{
    private readonly ITimeSlotRepository _timeSlotRepository;
    private readonly IMapper _mapper;

    public GetTimeSlotQueryHandler(ITimeSlotRepository timeSlotRepository, IMapper mapper)
    {
        _timeSlotRepository = timeSlotRepository;
        _mapper = mapper;
    }

    public async Task<TimeSlotDto> Handle(GetTimeSlotQuery request, CancellationToken cancellationToken)
    {
        var slot = await _timeSlotRepository.GetByIdAsync(request.SlotId, cancellationToken);
        if (slot == null)
            throw new NotFoundException(nameof(TimeSlot), request.SlotId);

        return _mapper.Map<TimeSlotDto>(slot);
    }
}
