using CourtManager.Application.Interfaces;
using MediatR;

namespace CourtManager.Application.Features.FieldSchedules.Queries;

public record GetFieldScheduleQuery(Guid FieldId) : IRequest<List<FieldScheduleDto>>;

public class GetFieldScheduleQueryHandler
    : IRequestHandler<GetFieldScheduleQuery, List<FieldScheduleDto>>
{
    private readonly IFieldScheduleRepository _scheduleRepository;

    public GetFieldScheduleQueryHandler(IFieldScheduleRepository scheduleRepository)
    {
        _scheduleRepository = scheduleRepository;
    }

    public async Task<List<FieldScheduleDto>> Handle(
        GetFieldScheduleQuery request,
        CancellationToken cancellationToken)
    {
        var rows = await _scheduleRepository.GetByFieldIdAsync(request.FieldId, cancellationToken);
        return rows
            .OrderBy(s => s.DayOfWeek)
            .Select(s => new FieldScheduleDto
            {
                ScheduleId = s.ScheduleId,
                FieldId = s.FieldId,
                DayOfWeek = s.DayOfWeek,
                OpenTime = s.OpenTime.ToString("HH:mm"),
                CloseTime = s.CloseTime.ToString("HH:mm"),
                SlotDurationMinutes = s.SlotDurationMinutes,
                IsActive = s.IsActive
            })
            .ToList();
    }
}
