using CourtManager.Application.DTOs;
using MediatR;

namespace CourtManager.Application.Features.Venues.Commands;

public record CreateVenueCommand(
    Guid OwnerId,
    string VenueName,
    string Address,
    decimal Latitude,
    decimal Longitude,
    string Description,
    string OpeningHours,
    string PhoneContact
) : IRequest<VenueDto>;
