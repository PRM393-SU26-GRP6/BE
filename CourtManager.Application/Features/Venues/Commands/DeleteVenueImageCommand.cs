using MediatR;

namespace CourtManager.Application.Features.Venues.Commands;

public record DeleteVenueImageCommand(
    Guid VenueId,
    Guid ImageId,
    Guid OwnerId
) : IRequest<bool>;
