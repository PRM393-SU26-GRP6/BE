using AutoMapper;
using CourtManager.Application.DTOs;
using CourtManager.Domain.Interfaces;
using MediatR;

namespace CourtManager.Application.Features.Venues.Queries;

public class GetVenueImagesQueryHandler : IRequestHandler<GetVenueImagesQuery, IEnumerable<VenueImageDto>>
{
    private readonly IVenueImageRepository _imageRepository;
    private readonly IMapper _mapper;

    public GetVenueImagesQueryHandler(IVenueImageRepository imageRepository, IMapper mapper)
    {
        _imageRepository = imageRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<VenueImageDto>> Handle(GetVenueImagesQuery request, CancellationToken cancellationToken)
    {
        var images = await _imageRepository.GetImagesByVenueIdAsync(request.VenueId, cancellationToken);
        return _mapper.Map<IEnumerable<VenueImageDto>>(images);
    }
}
