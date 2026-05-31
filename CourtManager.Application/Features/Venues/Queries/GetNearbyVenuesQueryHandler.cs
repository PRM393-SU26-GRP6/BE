using AutoMapper;
using CourtManager.Application.DTOs;
using CourtManager.Domain.Interfaces;
using MediatR;

namespace CourtManager.Application.Features.Venues.Queries;

public class GetNearbyVenuesQueryHandler : IRequestHandler<GetNearbyVenuesQuery, IEnumerable<VenueDto>>
{
    private readonly IVenueRepository _venueRepository;
    private readonly IMapper _mapper;

    public GetNearbyVenuesQueryHandler(IVenueRepository venueRepository, IMapper mapper)
    {
        _venueRepository = venueRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<VenueDto>> Handle(GetNearbyVenuesQuery request, CancellationToken cancellationToken)
    {
        var nearbyVenues = await _venueRepository.GetNearbyVenuesAsync(
            request.Latitude,
            request.Longitude,
            request.RadiusInKm,
            cancellationToken);

        var venueDtos = new List<VenueDto>();
        
        foreach (var item in nearbyVenues)
        {
            var dto = _mapper.Map<VenueDto>(item.Venue);
            dto.Distance = Math.Round(item.Distance, 2);
            venueDtos.Add(dto);
        }

        return venueDtos;
    }
}
