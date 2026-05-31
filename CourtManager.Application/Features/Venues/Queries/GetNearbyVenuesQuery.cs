using CourtManager.Application.DTOs;
using MediatR;

namespace CourtManager.Application.Features.Venues.Queries;

public class GetNearbyVenuesQuery : IRequest<IEnumerable<VenueDto>>
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public double RadiusInKm { get; set; } = 5.0; 
}
