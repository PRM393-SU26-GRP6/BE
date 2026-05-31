using CourtManager.Application.DTOs;
using MediatR;

namespace CourtManager.Application.Features.Venues.Commands;

/// <summary>
/// Updates an existing venue. Only the owner of the venue may update it.
/// OwnerId is resolved from the JWT, not from the request body.
/// </summary>
public record UpdateVenueCommand(
    Guid VenueId,
    Guid OwnerId,
    string VenueName,
    string Address,
    decimal Latitude,
    decimal Longitude,
    string Description,
    string OpeningHours,
    string PhoneContact
) : IRequest<VenueDto>;
