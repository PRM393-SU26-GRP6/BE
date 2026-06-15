using CourtManager.Application.Exceptions;
using CourtManager.Application.Features.FieldSchedules;
using CourtManager.Application.Interfaces;
using CourtManager.Domain.Entities;
using MediatR;

namespace CourtManager.Application.Features.FieldSchedules.Queries;

public record GetFieldWeekScheduleQuery(Guid FieldId)
    : IRequest<FieldWeekScheduleResponseDto>;

public class GetFieldWeekScheduleQueryHandler
    : IRequestHandler<GetFieldWeekScheduleQuery, FieldWeekScheduleResponseDto>
{
    private readonly IFieldScheduleRepository _scheduleRepository;
    private readonly IFootballFieldRepository _fieldRepository;

    private static readonly string[] DayNames =
    {
        "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday"
    };

    public GetFieldWeekScheduleQueryHandler(
        IFieldScheduleRepository scheduleRepository,
        IFootballFieldRepository fieldRepository)
    {
        _scheduleRepository = scheduleRepository;
        _fieldRepository = fieldRepository;
    }

    public async Task<FieldWeekScheduleResponseDto> Handle(
        GetFieldWeekScheduleQuery request,
        CancellationToken cancellationToken)
    {
        var field = await _fieldRepository.GetByIdAsync(request.FieldId, cancellationToken);
        if (field == null)
            throw new NotFoundException(nameof(FootballField), request.FieldId);

        var allRows = await _scheduleRepository.GetByFieldIdAsync(request.FieldId, cancellationToken);
        var byDay = allRows.ToDictionary(s => s.DayOfWeek);

        // Return Mon-Fri (day-of-week 1..5)
        var days = Enumerable.Range(1, 5).Select(dow =>
        {
            if (byDay.TryGetValue(dow, out var schedule) && schedule.IsActive)
            {
                return new FieldWeekScheduleDto
                {
                    DayOfWeek = dow,
                    DayName = DayNames[dow],
                    IsOpen = true,
                    OpenTime = schedule.OpenTime.ToString("HH:mm"),
                    CloseTime = schedule.CloseTime.ToString("HH:mm"),
                    SlotDurationMinutes = schedule.SlotDurationMinutes
                };
            }
            return new FieldWeekScheduleDto
            {
                DayOfWeek = dow,
                DayName = DayNames[dow],
                IsOpen = false,
                OpenTime = null,
                CloseTime = null,
                SlotDurationMinutes = 60
            };
        }).ToList();

        return new FieldWeekScheduleResponseDto
        {
            FieldId = request.FieldId,
            Days = days
        };
    }
}
