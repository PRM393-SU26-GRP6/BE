using AutoMapper;
using CourtManager.Application.DTOs;
using CourtManager.Domain.Interfaces;
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
            skip, 
            take, 
            cancellationToken);

        var totalItems = await _venueRepository.GetTotalCountAsync(
            request.Q, 
            request.PriceMin, 
            request.PriceMax, 
            request.MinRating, 
            cancellationToken);

        var venueDtos = _mapper.Map<IEnumerable<VenueDto>>(venues);

        return new PagedResult<VenueDto>(venueDtos, totalItems, request.Page, request.PageSize);
    }
}
