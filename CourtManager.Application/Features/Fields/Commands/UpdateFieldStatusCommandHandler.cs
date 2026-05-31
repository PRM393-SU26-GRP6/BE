using CourtManager.Application.Exceptions;
using CourtManager.Domain.Interfaces;
using MediatR;

namespace CourtManager.Application.Features.Fields.Commands;

public class UpdateFieldStatusCommandHandler : IRequestHandler<UpdateFieldStatusCommand, UpdateFieldStatusResult>
{
    private readonly IFootballFieldRepository _fieldRepository;
    private readonly IVenueRepository _venueRepository;

    public UpdateFieldStatusCommandHandler(
        IFootballFieldRepository fieldRepository,
        IVenueRepository venueRepository)
    {
        _fieldRepository = fieldRepository;
        _venueRepository = venueRepository;
    }

    public async Task<UpdateFieldStatusResult> Handle(UpdateFieldStatusCommand request, CancellationToken cancellationToken)
    {
        var field = await _fieldRepository.GetByIdAsync(request.FieldId, cancellationToken);
        if (field == null || field.IsDeleted)
        {
            throw new NotFoundException("FootballField", request.FieldId);
        }

        var venue = await _venueRepository.GetByIdAsync(field.VenueId, cancellationToken);
        if (venue == null || venue.OwnerId != request.OwnerId)
        {
            throw new ForbiddenException("You are not the owner of the venue this field belongs to.");
        }

        field.IsActive = request.IsActive;
        field.UpdatedAt = DateTime.UtcNow;

        await _fieldRepository.UpdateAsync(field, cancellationToken);
        await _fieldRepository.SaveChangesAsync(cancellationToken);

        var action = request.IsActive ? "activated" : "deactivated";
        return new UpdateFieldStatusResult(field.Id, field.IsActive, $"Field {action} successfully.");
    }
}
