using CourtManager.Application.Exceptions;
using CourtManager.Application.Interfaces;
using CourtManager.Domain.Interfaces;
using MediatR;

namespace CourtManager.Application.Features.Venues.Commands;

public class DeleteVenueImageCommandHandler : IRequestHandler<DeleteVenueImageCommand, bool>
{
    private readonly IVenueRepository _venueRepository;
    private readonly IVenueImageRepository _venueImageRepository;
    private readonly IStorageService _storageService;

    public DeleteVenueImageCommandHandler(
        IVenueRepository venueRepository,
        IVenueImageRepository venueImageRepository,
        IStorageService storageService)
    {
        _venueRepository = venueRepository;
        _venueImageRepository = venueImageRepository;
        _storageService = storageService;
    }

    public async Task<bool> Handle(DeleteVenueImageCommand request, CancellationToken cancellationToken)
    {
        var venue = await _venueRepository.GetByIdAsync(request.VenueId, cancellationToken);
        if (venue == null || venue.IsDeleted)
        {
            throw new NotFoundException("Venue", request.VenueId);
        }

        if (venue.OwnerId != request.OwnerId)
        {
            throw new ForbiddenException("You are not the owner of this venue.");
        }

        var image = await _venueImageRepository.GetByIdAsync(request.ImageId, cancellationToken);
        if (image == null || image.VenueId != request.VenueId)
        {
            throw new NotFoundException("VenueImage", request.ImageId);
        }

        // Delete from Cloudflare R2
        await _storageService.DeleteFileAsync(image.ImageUrl, cancellationToken);

        // Delete from DB
        await _venueImageRepository.DeleteAsync(image, cancellationToken);
        await _venueImageRepository.SaveChangesAsync(cancellationToken);

        return true;
    }
}
