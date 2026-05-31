using AutoMapper;
using CourtManager.Application.DTOs;
using CourtManager.Domain.Interfaces;
using MediatR;

namespace CourtManager.Application.Features.Amenities.Queries;

public class GetAllAmenitiesQueryHandler : IRequestHandler<GetAllAmenitiesQuery, IEnumerable<AmenityDto>>
{
    private readonly IAmenityRepository _amenityRepository;
    private readonly IMapper _mapper;

    public GetAllAmenitiesQueryHandler(IAmenityRepository amenityRepository, IMapper mapper)
    {
        _amenityRepository = amenityRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<AmenityDto>> Handle(GetAllAmenitiesQuery request, CancellationToken cancellationToken)
    {
        var amenities = await _amenityRepository.GetAllAsync(cancellationToken);
        return _mapper.Map<IEnumerable<AmenityDto>>(amenities);
    }
}
