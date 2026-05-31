using AutoMapper;
using CourtManager.Application.DTOs;
using CourtManager.Application.Exceptions;
using CourtManager.Domain.Interfaces;
using MediatR;

namespace CourtManager.Application.Features.Venues.Commands;

public class UpdateVenueCommandHandler : IRequestHandler<UpdateVenueCommand, VenueDto>
{
    private readonly IVenueRepository _venueRepository;
    private readonly IMapper _mapper;

    public UpdateVenueCommandHandler(IVenueRepository venueRepository, IMapper mapper)
    {
        _venueRepository = venueRepository;
        _mapper = mapper;
    }

    public async Task<VenueDto> Handle(UpdateVenueCommand request, CancellationToken cancellationToken)
    {
        var venue = await _venueRepository.GetByIdAsync(request.VenueId, cancellationToken);
        if (venue == null || venue.IsDeleted)
        {
            throw new NotFoundException("Venue", request.VenueId);
        }

        // Only the owner of this venue may update it
        if (venue.OwnerId != request.OwnerId)
        {
            throw new ForbiddenException("You are not the owner of this venue.");
        }

        venue.VenueName = request.VenueName;
        venue.Address = request.Address;
        venue.Latitude = request.Latitude;
        venue.Longitude = request.Longitude;
        venue.Description = request.Description;
        venue.OpeningHours = request.OpeningHours;
        venue.PhoneContact = request.PhoneContact;
        venue.UpdatedAt = DateTime.UtcNow;

        await _venueRepository.UpdateAsync(venue, cancellationToken);
        await _venueRepository.SaveChangesAsync(cancellationToken);

        // Reload with navigation properties so aggregate fields (owner, rating, price) are populated
        var full = await _venueRepository.GetVenueByIdAsync(venue.VenueId, cancellationToken);
        return _mapper.Map<VenueDto>(full ?? venue);
    }
}
