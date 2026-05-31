using CourtManager.Application.DTOs;
using MediatR;

namespace CourtManager.Application.Features.Venues.Queries;

public record GetVenueByIdQuery(Guid VenueId) : IRequest<VenueDetailDto?>;
