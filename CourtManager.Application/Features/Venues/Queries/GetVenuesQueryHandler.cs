using AutoMapper;
using CourtManager.Application.DTOs;
using CourtManager.Application.Interfaces;
using MediatR;

namespace CourtManager.Application.Features.Venues.Queries;

public class GetVenuesQueryHandler : IRequestHandler<GetVenuesQuery, PagedResult<VenueDto>>
{
    private readonly IVenueRepository _venueRepository;
    private readonly IMapper _mapper;

    public GetVenuesQueryHandler(IVenueRepository venueRepository, IMapper mapper)
    {
        _venueRepository = venueRepository;
        _mapper = mapper;
    }

    public async Task<PagedResult<VenueDto>> Handle(GetVenuesQuery request, CancellationToken cancellationToken)
    {
        var parsedAmenityIds = new List<Guid>();
        if (!string.IsNullOrEmpty(request.AmenityIds))
        {
            parsedAmenityIds = request.AmenityIds
                .Split(',')
                .Select(id => Guid.TryParse(id, out var parsedId) ? parsedId : Guid.Empty)
                .Where(id => id != Guid.Empty)
                .ToList();
        }

        var skip = (request.Page - 1) * request.PageSize;
        var take = request.PageSize;

        var venues = await _venueRepository.GetVenuesAsync(
            request.Q, 
            request.PriceMin, 
            request.PriceMax, 
            request.MinRating, 
            request.UserLatitude,
            request.UserLongitude,
            request.RadiusInKm,
            skip, 
            take, 
            cancellationToken);

        var totalItems = await _venueRepository.GetTotalCountAsync(
            request.Q, 
            request.PriceMin, 
            request.PriceMax, 
            request.MinRating, 
            request.UserLatitude,
            request.UserLongitude,
            request.RadiusInKm,
            cancellationToken);

        var venueDtos = _mapper.Map<List<VenueDto>>(venues);

        if (request.UserLatitude.HasValue && request.UserLongitude.HasValue)
        {
            foreach (var dto in venueDtos)
            {
                dto.Distance = CalculateDistance(request.UserLatitude.Value, request.UserLongitude.Value, dto.Latitude, dto.Longitude);
            }
        }

        return new PagedResult<VenueDto>(venueDtos, totalItems, request.Page, request.PageSize);
    }

    private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
    {
        var R = 6371; // Radius of the earth in km
        var dLat = Deg2Rad(lat2 - lat1);
        var dLon = Deg2Rad(lon2 - lon1);
        var a = 
            Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
            Math.Cos(Deg2Rad(lat1)) * Math.Cos(Deg2Rad(lat2)) * 
            Math.Sin(dLon / 2) * Math.Sin(dLon / 2); 
        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a)); 
        var d = R * c; // Distance in km
        return Math.Round(d, 2);
    }

    private double Deg2Rad(double deg)
    {
        return deg * (Math.PI / 180);
    }
}
