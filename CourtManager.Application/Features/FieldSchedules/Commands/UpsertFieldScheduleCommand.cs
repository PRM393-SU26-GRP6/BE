using CourtManager.Application.Exceptions;
using CourtManager.Application.Interfaces;
using MediatR;

namespace CourtManager.Application.Features.FieldSchedules.Commands;

public record UpsertFieldScheduleCommand(
    Guid OwnerId,
    Guid FieldId,
    UpsertFieldScheduleDto Request
) : IRequest<List<FieldScheduleDto>>;

public class UpsertFieldScheduleCommandHandler
    : IRequestHandler<UpsertFieldScheduleCommand, List<FieldScheduleDto>>
{
    private readonly IFieldScheduleRepository _scheduleRepository;
    private readonly IFootballFieldRepository _fieldRepository;
    private readonly IVenueRepository _venueRepository;

    public UpsertFieldScheduleCommandHandler(
        IFieldScheduleRepository scheduleRepository,
        IFootballFieldRepository fieldRepository,
        IVenueRepository venueRepository)
    {
        _scheduleRepository = scheduleRepository;
        _fieldRepository = fieldRepository;
        _venueRepository = venueRepository;
    }

    public async Task<List<FieldScheduleDto>> Handle(
        UpsertFieldScheduleCommand request,
        CancellationToken cancellationToken)
    {
        var field = await _fieldRepository.GetByIdAsync(request.FieldId, cancellationToken);
        if (field == null)
            throw new NotFoundException(nameof(Domain.Entities.FootballField), request.FieldId);

        var venue = await _venueRepository.GetByIdAsync(field.VenueId, cancellationToken);
        if (venue == null || venue.OwnerId != request.OwnerId)
            throw new ValidationException("Only the venue owner can update this field's schedule.");

        var now = DateTime.UtcNow;

        var rows = request.Request.Rows ?? new List<FieldScheduleRowDto>();

        foreach (var r in rows)
        {
            if (r.DayOfWeek < 0 || r.DayOfWeek > 6)
                throw new ValidationException($"DayOfWeek must be 0..6 (got {r.DayOfWeek}).");
            if (!TimeOnly.TryParse(r.OpenTime, out _) || !TimeOnly.TryParse(r.CloseTime, out _))
                throw new ValidationException($"OpenTime/CloseTime on day {r.DayOfWeek} must be valid HH:mm.");
            if (r.SlotDurationMinutes <= 0 || r.SlotDurationMinutes > 720)
                throw new ValidationException($"SlotDurationMinutes on day {r.DayOfWeek} must be 1..720.");
        }

        var dupes = rows.GroupBy(r => r.DayOfWeek).Where(g => g.Count() > 1).Select(g => g.Key).ToList();
        if (dupes.Any())
            throw new ValidationException($"Duplicate DayOfWeek rows: {string.Join(",", dupes)}");

        var entities = rows.Select(r => new Domain.Entities.FieldSchedule
        {
            ScheduleId = Guid.NewGuid(),
            FieldId = request.FieldId,
            DayOfWeek = r.DayOfWeek,
            OpenTime = TimeOnly.Parse(r.OpenTime),
            CloseTime = TimeOnly.Parse(r.CloseTime),
            SlotDurationMinutes = r.SlotDurationMinutes,
            IsActive = r.IsActive,
            CreatedAt = now,
            UpdatedAt = now
        }).ToList();

        await _scheduleRepository.ReplaceAllForFieldAsync(request.FieldId, entities, cancellationToken);

        var saved = (await _scheduleRepository.GetByFieldIdAsync(request.FieldId, cancellationToken))
            .OrderBy(s => s.DayOfWeek)
            .ToList();

        return saved.Select(s => new FieldScheduleDto
        {
            ScheduleId = s.ScheduleId,
            FieldId = s.FieldId,
            DayOfWeek = s.DayOfWeek,
            OpenTime = s.OpenTime.ToString("HH:mm"),
            CloseTime = s.CloseTime.ToString("HH:mm"),
            SlotDurationMinutes = s.SlotDurationMinutes,
            IsActive = s.IsActive
        }).ToList();
    }
}
