using AutoMapper;
using CourtManager.Application.DTOs;
using CourtManager.Domain.Interfaces;
using MediatR;

namespace CourtManager.Application.Features.Venues.Queries;

public class GetOwnerVenuesQueryHandler : IRequestHandler<GetOwnerVenuesQuery, PagedResult<VenueDto>>
{
    private readonly IVenueRepository _venueRepository;
    private readonly IMapper _mapper;

    public GetOwnerVenuesQueryHandler(IVenueRepository venueRepository, IMapper mapper)
    {
        _venueRepository = venueRepository;
        _mapper = mapper;
    }

    public async Task<PagedResult<VenueDto>> Handle(GetOwnerVenuesQuery request, CancellationToken cancellationToken)
    {
        var skip = (request.Page - 1) * request.PageSize;

        var venues = await _venueRepository.GetOwnerVenuesAsync(
            request.OwnerId,
            request.IsActive,
            skip,
            request.PageSize,
            cancellationToken);

        var totalItems = await _venueRepository.GetOwnerVenuesCountAsync(
            request.OwnerId,
            request.IsActive,
            cancellationToken);

        var venueDtos = _mapper.Map<IEnumerable<VenueDto>>(venues);

        return new PagedResult<VenueDto>(venueDtos, totalItems, request.Page, request.PageSize);
    }
}
