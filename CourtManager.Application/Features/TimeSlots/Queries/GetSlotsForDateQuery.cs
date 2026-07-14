using CourtManager.Application.Exceptions;
using CourtManager.Application.Features.FieldSchedules;
using CourtManager.Application.Interfaces;
using CourtManager.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CourtManager.Application.Features.TimeSlots.Queries;

public record GetSlotsForDateQuery(
    Guid FieldId,
    DateTime Date  // caller passes a UTC date; only .Date is used
) : IRequest<List<SlotForDateDto>>;

public class GetSlotsForDateQueryHandler
    : IRequestHandler<GetSlotsForDateQuery, List<SlotForDateDto>>
{
    private readonly IFieldScheduleRepository _scheduleRepository;
    private readonly ITimeSlotRepository _timeSlotRepository;
    private readonly IFootballFieldRepository _fieldRepository;

    public GetSlotsForDateQueryHandler(
        IFieldScheduleRepository scheduleRepository,
        ITimeSlotRepository timeSlotRepository,
        IFootballFieldRepository fieldRepository)
    {
        _scheduleRepository = scheduleRepository;
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

        var dayOfWeek = (int)request.Date.DayOfWeek;
        var schedule = await _scheduleRepository.GetForDayAsync(request.FieldId, dayOfWeek, cancellationToken);

        if (schedule == null || !schedule.IsActive)
        {
            // Field closed that day — return empty list (200 OK with []) rather than 404
            return new List<SlotForDateDto>();
        }

        // Generate virtual slots in memory from OpenTime/CloseTime + SlotDurationMinutes
        var date = DateTime.SpecifyKind(request.Date.Date, DateTimeKind.Utc);
        var openUtc = date.Add(schedule.OpenTime.ToTimeSpan());
        var closeUtc = date.Add(schedule.CloseTime.ToTimeSpan());
        var step = TimeSpan.FromMinutes(schedule.SlotDurationMinutes);

        var virtualSlots = new List<SlotForDateDto>();
        for (var start = openUtc; start.Add(step) <= closeUtc; start = start.Add(step))
        {
            var end = start.Add(step);
            virtualSlots.Add(new SlotForDateDto
            {
                StartTime = start,
                EndTime = end,
                StartTimeOfDay = start.ToString("HH:mm"),
                EndTimeOfDay = end.ToString("HH:mm"),
                Price = field.PricePerHour, // TODO (post-MVP): per-slot price override
                SlotStatus = "Available",
                SlotId = null
            });
        }

        // Overlay real TimeSlot rows for this date to surface SlotId + SlotStatus
        var realSlots = await _timeSlotRepository.GetAvailableSlotsAsync(
            request.FieldId, date, cancellationToken);

        // Index by StartTime (HH:mm) for O(1) overlay
        var realByKey = realSlots
            .Where(s => !s.IsDeleted)
            .ToDictionary(s => s.StartTime.ToString("HH:mm"), s => s, StringComparer.Ordinal);

        foreach (var v in virtualSlots)
        {
            if (realByKey.TryGetValue(v.StartTimeOfDay, out var real))
            {
                v.SlotId = real.SlotId;
                v.SlotStatus = real.SlotStatus.ToString();
                v.Price = real.Price;
            }
        }

        return virtualSlots;
    }
}
