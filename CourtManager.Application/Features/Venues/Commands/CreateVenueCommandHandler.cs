using AutoMapper;
using CourtManager.Application.DTOs;
using CourtManager.Domain.Entities;
using CourtManager.Domain.Interfaces;
using MediatR;

namespace CourtManager.Application.Features.Venues.Commands;

public class CreateVenueCommandHandler : IRequestHandler<CreateVenueCommand, VenueDto>
{
    private readonly IVenueRepository _venueRepository;
    private readonly IMapper _mapper;

    public CreateVenueCommandHandler(IVenueRepository venueRepository, IMapper mapper)
    {
        _venueRepository = venueRepository;
        _mapper = mapper;
    }

    public async Task<VenueDto> Handle(CreateVenueCommand request, CancellationToken cancellationToken)
    {
        var venue = new Venue
        {
            VenueId = Guid.NewGuid(),
            OwnerId = request.OwnerId,
            VenueName = request.VenueName,
            Address = request.Address,
            Latitude = request.Latitude,
            Longitude = request.Longitude,
            Description = request.Description,
            OpeningHours = request.OpeningHours,
            PhoneContact = request.PhoneContact,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        var createdVenue = await _venueRepository.AddAsync(venue, cancellationToken);
        await _venueRepository.SaveChangesAsync(cancellationToken);

        return _mapper.Map<VenueDto>(createdVenue);
    }
}
