using AutoMapper;
using CourtManager.Application.DTOs;
using CourtManager.Domain.Interfaces;
using MediatR;

namespace CourtManager.Application.Features.Venues.Queries;

public class GetVenueFieldsQueryHandler : IRequestHandler<GetVenueFieldsQuery, IEnumerable<FootballFieldDto>>
{
    private readonly IFootballFieldRepository _fieldRepository;
    private readonly IMapper _mapper;

    public GetVenueFieldsQueryHandler(IFootballFieldRepository fieldRepository, IMapper mapper)
    {
        _fieldRepository = fieldRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<FootballFieldDto>> Handle(GetVenueFieldsQuery request, CancellationToken cancellationToken)
    {
        var fields = await _fieldRepository.GetFieldsByVenueIdAsync(request.VenueId, cancellationToken);
        return _mapper.Map<IEnumerable<FootballFieldDto>>(fields);
    }
}
