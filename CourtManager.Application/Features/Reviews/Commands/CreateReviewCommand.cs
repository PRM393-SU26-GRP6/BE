using CourtManager.Application.DTOs;
using MediatR;

namespace CourtManager.Application.Features.Reviews.Commands;

public record CreateReviewCommand(
    Guid VenueId,
    Guid BookingId,
    int Rating,
    string? Comment,
    Guid UserId
) : IRequest<ReviewDto>;
