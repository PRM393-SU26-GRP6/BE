using CourtManager.Application.DTOs;
using MediatR;

namespace CourtManager.Application.Features.Amenities.Queries;

public record GetAllAmenitiesQuery() : IRequest<IEnumerable<AmenityDto>>;
