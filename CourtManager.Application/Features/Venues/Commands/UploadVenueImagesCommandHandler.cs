using CourtManager.Application.Interfaces;
using CourtManager.Domain.Entities;
using CourtManager.Domain.Interfaces;
using MediatR;

namespace CourtManager.Application.Features.Venues.Commands;

public class UploadVenueImagesCommandHandler : IRequestHandler<UploadVenueImagesCommand, List<string>>
{
    private readonly IVenueRepository _venueRepository;
    private readonly IVenueImageRepository _venueImageRepository;
    private readonly IStorageService _storageService;

    public UploadVenueImagesCommandHandler(
        IVenueRepository venueRepository,
        IVenueImageRepository venueImageRepository,
        IStorageService storageService)
    {
        _venueRepository = venueRepository;
        _venueImageRepository = venueImageRepository;
        _storageService = storageService;
    }

    public async Task<List<string>> Handle(UploadVenueImagesCommand request, CancellationToken cancellationToken)
    {
        var venue = await _venueRepository.GetByIdAsync(request.VenueId, cancellationToken);
        if (venue == null)
        {
            throw new Exception("Venue not found");
        }

        // Only owner or admin should upload images for this venue
        // Assuming Owner validation was partly done, but strictly check:
        if (venue.OwnerId != request.UserId)
        {
            // Note: In real logic we might check if user is Admin, but for now we enforce Owner ownership
            throw new Exception("You are not the owner of this venue");
        }

        var uploadedUrls = new List<string>();
        var venueImages = new List<VenueImage>();

        foreach (var image in request.Images)
        {
            if (image.Content.Length > 0)
            {
                var folderName = $"venues/{venue.VenueId}";
                var url = await _storageService.UploadFileAsync(image.Content, image.FileName, image.ContentType, folderName, cancellationToken);
                
                uploadedUrls.Add(url);
                venueImages.Add(new VenueImage
                {
                    ImageId = Guid.NewGuid(),
                    VenueId = venue.VenueId,
                    ImageUrl = url
                });
            }
        }

        if (venueImages.Any())
        {
            await _venueImageRepository.AddMultipleAsync(venueImages, cancellationToken);
            await _venueImageRepository.SaveChangesAsync(cancellationToken);
        }

        return uploadedUrls;
    }
}
