using CourtManager.Application.DTOs;
using MediatR;

namespace CourtManager.Application.Features.Venues.Queries;

public record GetVenueImagesQuery(Guid VenueId) : IRequest<IEnumerable<VenueImageDto>>;
