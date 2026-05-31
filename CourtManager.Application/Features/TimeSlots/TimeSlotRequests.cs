using AutoMapper;
using CourtManager.Application.DTOs;
using CourtManager.Application.Exceptions;
using CourtManager.Domain.Entities;
using CourtManager.Domain.Enums;
using CourtManager.Domain.Interfaces;
using MediatR;

namespace CourtManager.Application.Features.TimeSlots;

public record GetSlotsByFieldQuery(Guid FieldId) : IRequest<IEnumerable<TimeSlotDto>>;
public record GetTimeSlotByIdQuery(Guid SlotId) : IRequest<TimeSlotDto>;
public record CreateTimeSlotCommand(Guid OwnerId, TimeSlotDto Slot) : IRequest<TimeSlotDto>;
public record UpdateTimeSlotCommand(Guid OwnerId, Guid SlotId, TimeSlotDto Slot) : IRequest<TimeSlotDto>;

public class GetSlotsByFieldQueryHandler : IRequestHandler<GetSlotsByFieldQuery, IEnumerable<TimeSlotDto>>
{
    private readonly ITimeSlotRepository _timeSlotRepository;
    private readonly IMapper _mapper;

    public GetSlotsByFieldQueryHandler(ITimeSlotRepository timeSlotRepository, IMapper mapper)
    {
        _timeSlotRepository = timeSlotRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<TimeSlotDto>> Handle(GetSlotsByFieldQuery request, CancellationToken cancellationToken)
    {
        var slots = await _timeSlotRepository.GetSlotsByFieldIdAsync(request.FieldId, cancellationToken);
        return _mapper.Map<IEnumerable<TimeSlotDto>>(slots);
    }
}

public class GetTimeSlotByIdQueryHandler : IRequestHandler<GetTimeSlotByIdQuery, TimeSlotDto>
{
    private readonly ITimeSlotRepository _timeSlotRepository;
    private readonly IMapper _mapper;

    public GetTimeSlotByIdQueryHandler(ITimeSlotRepository timeSlotRepository, IMapper mapper)
    {
        _timeSlotRepository = timeSlotRepository;
        _mapper = mapper;
    }

    public async Task<TimeSlotDto> Handle(GetTimeSlotByIdQuery request, CancellationToken cancellationToken)
    {
        var slot = await _timeSlotRepository.GetByIdAsync(request.SlotId, cancellationToken);
        if (slot == null)
        {
            throw new NotFoundException(nameof(TimeSlot), request.SlotId);
        }

        return _mapper.Map<TimeSlotDto>(slot);
    }
}

public class CreateTimeSlotCommandHandler : IRequestHandler<CreateTimeSlotCommand, TimeSlotDto>
{
    private readonly IFootballFieldRepository _fieldRepository;
    private readonly ITimeSlotRepository _timeSlotRepository;
    private readonly IMapper _mapper;

    public CreateTimeSlotCommandHandler(IFootballFieldRepository fieldRepository, ITimeSlotRepository timeSlotRepository, IMapper mapper)
    {
        _fieldRepository = fieldRepository;
        _timeSlotRepository = timeSlotRepository;
        _mapper = mapper;
    }

    public async Task<TimeSlotDto> Handle(CreateTimeSlotCommand request, CancellationToken cancellationToken)
    {
        var field = await _fieldRepository.GetByIdAsync(request.Slot.FieldId, cancellationToken);
        if (field == null)
        {
            throw new NotFoundException(nameof(FootballField), request.Slot.FieldId);
        }

        if (field.Venue?.OwnerId != request.OwnerId)
        {
            throw new ValidationException("Only the venue owner can create slots for this field.");
        }

        ValidateSlot(request.Slot.StartTime, request.Slot.EndTime);

        var slot = new TimeSlot
        {
            SlotId = Guid.NewGuid(),
            FieldId = request.Slot.FieldId,
            StartTime = request.Slot.StartTime,
            EndTime = request.Slot.EndTime,
            Price = request.Slot.Price > 0 ? request.Slot.Price : field.PricePerHour,
            SlotStatus = ParseSlotStatus(request.Slot.SlotStatus, SlotStatus.Available),
            CreatedAt = DateTime.UtcNow
        };

        await _timeSlotRepository.AddAsync(slot, cancellationToken);
        await _timeSlotRepository.SaveChangesAsync(cancellationToken);
        return _mapper.Map<TimeSlotDto>(slot);
    }

    public static void ValidateSlot(DateTime startTime, DateTime endTime)
    {
        if (endTime <= startTime)
        {
            throw new ValidationException("EndTime must be after StartTime.");
        }
    }

    public static SlotStatus ParseSlotStatus(string? status, SlotStatus defaultStatus)
    {
        return string.IsNullOrWhiteSpace(status)
            ? defaultStatus
            : Enum.TryParse<SlotStatus>(status, true, out var parsed)
                ? parsed
                : throw new ValidationException("SlotStatus must be Available, Locked, or Booked.");
    }
}

public class UpdateTimeSlotCommandHandler : IRequestHandler<UpdateTimeSlotCommand, TimeSlotDto>
{
    private readonly ITimeSlotRepository _timeSlotRepository;
    private readonly IMapper _mapper;

    public UpdateTimeSlotCommandHandler(ITimeSlotRepository timeSlotRepository, IMapper mapper)
    {
        _timeSlotRepository = timeSlotRepository;
        _mapper = mapper;
    }

    public async Task<TimeSlotDto> Handle(UpdateTimeSlotCommand request, CancellationToken cancellationToken)
    {
        var slot = await _timeSlotRepository.GetByIdAsync(request.SlotId, cancellationToken);
        if (slot == null)
        {
            throw new NotFoundException(nameof(TimeSlot), request.SlotId);
        }

        if (slot.Field?.Venue?.OwnerId != request.OwnerId)
        {
            throw new ValidationException("Only the venue owner can update this slot.");
        }

        CreateTimeSlotCommandHandler.ValidateSlot(request.Slot.StartTime, request.Slot.EndTime);

        slot.StartTime = request.Slot.StartTime;
        slot.EndTime = request.Slot.EndTime;
        slot.Price = request.Slot.Price;
        slot.SlotStatus = CreateTimeSlotCommandHandler.ParseSlotStatus(request.Slot.SlotStatus, slot.SlotStatus);
        slot.UpdatedAt = DateTime.UtcNow;

        await _timeSlotRepository.UpdateAsync(slot, cancellationToken);
        await _timeSlotRepository.SaveChangesAsync(cancellationToken);

        return _mapper.Map<TimeSlotDto>(slot);
    }
}
