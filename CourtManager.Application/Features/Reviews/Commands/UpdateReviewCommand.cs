using CourtManager.Application.DTOs;
using MediatR;

namespace CourtManager.Application.Features.Reviews.Commands;

public record UpdateReviewCommand(
    Guid ReviewId,
    int Rating,
    string? Comment,
    Guid UserId
) : IRequest<ReviewDto>;
