using AutoMapper;
using CourtManager.Application.DTOs;
using CourtManager.Domain.Interfaces;
using MediatR;

namespace CourtManager.Application.Features.Amenities;

public record GetAmenitiesQuery : IRequest<IEnumerable<AmenityDto>>;

public class GetAmenitiesQueryHandler : IRequestHandler<GetAmenitiesQuery, IEnumerable<AmenityDto>>
{
    private readonly IAmenityRepository _amenityRepository;
    private readonly IMapper _mapper;

    public GetAmenitiesQueryHandler(IAmenityRepository amenityRepository, IMapper mapper)
    {
        _amenityRepository = amenityRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<AmenityDto>> Handle(GetAmenitiesQuery request, CancellationToken cancellationToken)
    {
        var amenities = await _amenityRepository.GetAllAsync(cancellationToken);
        return _mapper.Map<IEnumerable<AmenityDto>>(amenities);
    }
}

