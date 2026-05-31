using AutoMapper;
using CourtManager.Application.DTOs;
using CourtManager.Application.Exceptions;
using CourtManager.Domain.Interfaces;
using MediatR;

namespace CourtManager.Application.Features.Fields.Commands;

public class UpdateFieldCommandHandler : IRequestHandler<UpdateFieldCommand, FootballFieldDto>
{
    private readonly IFootballFieldRepository _fieldRepository;
    private readonly IVenueRepository _venueRepository;
    private readonly IMapper _mapper;

    public UpdateFieldCommandHandler(
        IFootballFieldRepository fieldRepository,
        IVenueRepository venueRepository,
        IMapper mapper)
    {
        _fieldRepository = fieldRepository;
        _venueRepository = venueRepository;
        _mapper = mapper;
    }

    public async Task<FootballFieldDto> Handle(UpdateFieldCommand request, CancellationToken cancellationToken)
    {
        var field = await _fieldRepository.GetByIdAsync(request.FieldId, cancellationToken);
        if (field == null || field.IsDeleted)
        {
            throw new NotFoundException("FootballField", request.FieldId);
        }

        // Verify the caller owns the parent venue
        var venue = await _venueRepository.GetByIdAsync(field.VenueId, cancellationToken);
        if (venue == null || venue.OwnerId != request.OwnerId)
        {
            throw new ForbiddenException("You are not the owner of the venue this field belongs to.");
        }

        field.FieldName = request.FieldName;
        field.FieldType = request.FieldType;
        field.PricePerHour = request.PricePerHour;
        field.UpdatedAt = DateTime.UtcNow;

        await _fieldRepository.UpdateAsync(field, cancellationToken);
        await _fieldRepository.SaveChangesAsync(cancellationToken);

        return _mapper.Map<FootballFieldDto>(field);
    }
}
