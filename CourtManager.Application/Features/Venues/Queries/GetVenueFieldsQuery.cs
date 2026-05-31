using CourtManager.Application.DTOs;
using MediatR;

namespace CourtManager.Application.Features.Venues.Queries;

public record GetVenueFieldsQuery(Guid VenueId) : IRequest<IEnumerable<FootballFieldDto>>;
