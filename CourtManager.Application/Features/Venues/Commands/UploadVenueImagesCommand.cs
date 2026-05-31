using CourtManager.Application.DTOs;
using MediatR;

namespace CourtManager.Application.Features.Venues.Commands;

public record UploadVenueImagesCommand(
    Guid VenueId,
    Guid UserId,
    List<FileUploadDto> Images
) : IRequest<List<string>>;
