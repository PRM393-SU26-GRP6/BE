using AutoMapper;
using CourtManager.Application.DTOs;
using CourtManager.Domain.Interfaces;
using MediatR;

namespace CourtManager.Application.Features.Venues.Queries;

public class GetVenueByIdQueryHandler : IRequestHandler<GetVenueByIdQuery, VenueDetailDto?>
{
    private readonly IVenueRepository _venueRepository;
    private readonly IMapper _mapper;

    public GetVenueByIdQueryHandler(IVenueRepository venueRepository, IMapper mapper)
    {
        _venueRepository = venueRepository;
        _mapper = mapper;
    }

    public async Task<VenueDetailDto?> Handle(GetVenueByIdQuery request, CancellationToken cancellationToken)
    {
        var venue = await _venueRepository.GetVenueByIdAsync(request.VenueId, cancellationToken);

        if (venue == null)
            return null;

        return _mapper.Map<VenueDetailDto>(venue);
    }
}
