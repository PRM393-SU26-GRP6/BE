using AutoMapper;
using CourtManager.Application.DTOs;
using CourtManager.Domain.Interfaces;
using MediatR;

namespace CourtManager.Application.Features.Venues.Queries;

public class GetVenueAmenitiesQueryHandler : IRequestHandler<GetVenueAmenitiesQuery, IEnumerable<AmenityDto>>
{
    private readonly IVenueRepository _venueRepository;
    private readonly IMapper _mapper;

    public GetVenueAmenitiesQueryHandler(IVenueRepository venueRepository, IMapper mapper)
    {
        _venueRepository = venueRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<AmenityDto>> Handle(GetVenueAmenitiesQuery request, CancellationToken cancellationToken)
    {
        var amenities = await _venueRepository.GetVenueAmenitiesAsync(request.VenueId, cancellationToken);
        return _mapper.Map<IEnumerable<AmenityDto>>(amenities);
    }
}
