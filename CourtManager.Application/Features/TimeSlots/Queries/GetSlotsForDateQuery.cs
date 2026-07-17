using CourtManager.Application.Exceptions;
using CourtManager.Application.Features.FieldSchedules;
using CourtManager.Application.Interfaces;
using CourtManager.Domain.Entities;
using MediatR;

namespace CourtManager.Application.Features.TimeSlots.Queries;

public record GetSlotsForDateQuery(
    Guid FieldId,
    DateTime Date  // caller passes a UTC date; only .Date is used
) : IRequest<List<SlotForDateDto>>;

public class GetSlotsForDateQueryHandler
    : IRequestHandler<GetSlotsForDateQuery, List<SlotForDateDto>>
{
    private readonly ITimeSlotRepository _timeSlotRepository;
    private readonly IFootballFieldRepository _fieldRepository;

    public GetSlotsForDateQueryHandler(
        ITimeSlotRepository timeSlotRepository,
        IFootballFieldRepository fieldRepository)
    {
        _timeSlotRepository = timeSlotRepository;
        _fieldRepository = fieldRepository;
    }

    public async Task<List<SlotForDateDto>> Handle(
        GetSlotsForDateQuery request,
        CancellationToken cancellationToken)
    {
        if (request.FieldId == Guid.Empty)
            throw new ValidationException("FieldId is required.");

        var field = await _fieldRepository.GetByIdAsync(request.FieldId, cancellationToken);
        if (field == null)
            throw new NotFoundException(nameof(FootballField), request.FieldId);

        if (!field.IsActive)
            return [];

        var date = DateTime.SpecifyKind(request.Date.Date, DateTimeKind.Utc);
        var slots = await _timeSlotRepository.GetAvailableSlotsAsync(
            request.FieldId, date, cancellationToken);

        return slots.Select(slot => new SlotForDateDto
        {
            StartTime = date.Add(slot.StartTime.ToTimeSpan()),
            EndTime = date.Add(slot.EndTime.ToTimeSpan()),
            StartTimeOfDay = slot.StartTime.ToString("HH:mm"),
            EndTimeOfDay = slot.EndTime.ToString("HH:mm"),
            Price = slot.Price,
            SlotStatus = slot.SlotStatus.ToString(),
            SlotId = slot.SlotId
        }).ToList();
    }
}
