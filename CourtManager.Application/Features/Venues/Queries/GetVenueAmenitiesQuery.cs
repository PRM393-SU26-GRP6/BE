using CourtManager.Application.DTOs;
using MediatR;

namespace CourtManager.Application.Features.Venues.Queries;

public record GetVenueAmenitiesQuery(Guid VenueId) : IRequest<IEnumerable<AmenityDto>>;
