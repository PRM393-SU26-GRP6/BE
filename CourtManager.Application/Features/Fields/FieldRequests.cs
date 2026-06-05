using AutoMapper;
using CourtManager.Application.DTOs;
using CourtManager.Application.Exceptions;
using CourtManager.Domain.Entities;
using CourtManager.Domain.Enums;
using CourtManager.Application.Interfaces;
using MediatR;

namespace CourtManager.Application.Features.Fields;

public record GetFieldsByVenueQuery(Guid VenueId) : IRequest<IEnumerable<FootballFieldDto>>;
public record GetFieldByIdQuery(Guid FieldId) : IRequest<FootballFieldDto>;
public record CreateFieldCommand(Guid OwnerId, Guid VenueId, FootballFieldDto Field) : IRequest<FootballFieldDto>;
public record UpdateFieldCommand(Guid OwnerId, Guid FieldId, FootballFieldDto Field) : IRequest<FootballFieldDto>;
public record DeleteFieldCommand(Guid OwnerId, Guid FieldId) : IRequest<bool>;

public class GetFieldsByVenueQueryHandler : IRequestHandler<GetFieldsByVenueQuery, IEnumerable<FootballFieldDto>>
{
    private readonly IFootballFieldRepository _fieldRepository;
    private readonly IMapper _mapper;

    public GetFieldsByVenueQueryHandler(IFootballFieldRepository fieldRepository, IMapper mapper)
    {
        _fieldRepository = fieldRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<FootballFieldDto>> Handle(GetFieldsByVenueQuery request, CancellationToken cancellationToken)
    {
        var fields = await _fieldRepository.GetFieldsByVenueIdAsync(request.VenueId, cancellationToken);
        return _mapper.Map<IEnumerable<FootballFieldDto>>(fields);
    }
}

public class GetFieldByIdQueryHandler : IRequestHandler<GetFieldByIdQuery, FootballFieldDto>
{
    private readonly IFootballFieldRepository _fieldRepository;
    private readonly IMapper _mapper;

    public GetFieldByIdQueryHandler(IFootballFieldRepository fieldRepository, IMapper mapper)
    {
        _fieldRepository = fieldRepository;
        _mapper = mapper;
    }

    public async Task<FootballFieldDto> Handle(GetFieldByIdQuery request, CancellationToken cancellationToken)
    {
        var field = await _fieldRepository.GetByIdAsync(request.FieldId, cancellationToken);
        if (field == null)
        {
            throw new NotFoundException(nameof(FootballField), request.FieldId);
        }

        return _mapper.Map<FootballFieldDto>(field);
    }
}

public class CreateFieldCommandHandler : IRequestHandler<CreateFieldCommand, FootballFieldDto>
{
    private readonly IVenueRepository _venueRepository;
    private readonly IFootballFieldRepository _fieldRepository;
    private readonly IMapper _mapper;

    public CreateFieldCommandHandler(IVenueRepository venueRepository, IFootballFieldRepository fieldRepository, IMapper mapper)
    {
        _venueRepository = venueRepository;
        _fieldRepository = fieldRepository;
        _mapper = mapper;
    }

    public async Task<FootballFieldDto> Handle(CreateFieldCommand request, CancellationToken cancellationToken)
    {
        var venue = await _venueRepository.GetVenueByIdAsync(request.VenueId, cancellationToken);
        if (venue == null)
        {
            throw new NotFoundException(nameof(Venue), request.VenueId);
        }

        if (venue.OwnerId != request.OwnerId)
        {
            throw new ValidationException("Only the venue owner can create fields for this venue.");
        }

        var field = new FootballField
        {
            Id = Guid.NewGuid(),
            VenueId = request.VenueId,
            FieldName = request.Field.FieldName?.Trim() ?? string.Empty,
            Description = request.Field.Description?.Trim() ?? string.Empty,
            FieldType = ParseFieldType(request.Field.FieldType),
            PricePerHour = request.Field.PricePerHour,
            IsActive = request.Field.IsActive,
            CreatedAt = DateTime.UtcNow
        };

        await _fieldRepository.AddAsync(field, cancellationToken);
        await _fieldRepository.SaveChangesAsync(cancellationToken);

        var created = await _fieldRepository.GetByIdAsync(field.Id, cancellationToken) ?? field;
        return _mapper.Map<FootballFieldDto>(created);
    }

    public static FieldType ParseFieldType(string fieldType)
    {
        if (Enum.TryParse<FieldType>(fieldType, true, out var parsed))
        {
            return parsed;
        }

        return int.TryParse(fieldType, out var size) && Enum.IsDefined(typeof(FieldType), size)
            ? (FieldType)size
            : throw new ValidationException("FieldType must be FiveASide, SevenASide, ElevenASide, 5, 7, or 11.");
    }
}

public class UpdateFieldCommandHandler : IRequestHandler<UpdateFieldCommand, FootballFieldDto>
{
    private readonly IFootballFieldRepository _fieldRepository;
    private readonly IMapper _mapper;

    public UpdateFieldCommandHandler(IFootballFieldRepository fieldRepository, IMapper mapper)
    {
        _fieldRepository = fieldRepository;
        _mapper = mapper;
    }

    public async Task<FootballFieldDto> Handle(UpdateFieldCommand request, CancellationToken cancellationToken)
    {
        var field = await _fieldRepository.GetByIdAsync(request.FieldId, cancellationToken);
        if (field == null)
        {
            throw new NotFoundException(nameof(FootballField), request.FieldId);
        }

        if (field.Venue?.OwnerId != request.OwnerId)
        {
            throw new ValidationException("Only the venue owner can update this field.");
        }

        field.FieldName = request.Field.FieldName?.Trim() ?? string.Empty;
        field.Description = request.Field.Description?.Trim() ?? string.Empty;
        field.FieldType = CreateFieldCommandHandler.ParseFieldType(request.Field.FieldType);
        field.PricePerHour = request.Field.PricePerHour;
        field.IsActive = request.Field.IsActive;
        field.UpdatedAt = DateTime.UtcNow;

        await _fieldRepository.UpdateAsync(field, cancellationToken);
        await _fieldRepository.SaveChangesAsync(cancellationToken);
        return _mapper.Map<FootballFieldDto>(field);
    }
}

public class DeleteFieldCommandHandler : IRequestHandler<DeleteFieldCommand, bool>
{
    private readonly IFootballFieldRepository _fieldRepository;

    public DeleteFieldCommandHandler(IFootballFieldRepository fieldRepository)
    {
        _fieldRepository = fieldRepository;
    }

    public async Task<bool> Handle(DeleteFieldCommand request, CancellationToken cancellationToken)
    {
        var field = await _fieldRepository.GetByIdAsync(request.FieldId, cancellationToken);
        if (field == null)
        {
            throw new NotFoundException(nameof(FootballField), request.FieldId);
        }

        if (field.Venue?.OwnerId != request.OwnerId)
        {
            throw new ValidationException("Only the venue owner can delete this field.");
        }

        await _fieldRepository.DeleteAsync(field, cancellationToken);
        await _fieldRepository.SaveChangesAsync(cancellationToken);
        return true;
    }
}
