using CourtManager.Application.DTOs;
using MediatR;

namespace CourtManager.Application.Features.Reviews.Queries;

public record GetVenueReviewsQuery(
    Guid VenueId,
    int Page,
    int PageSize
) : IRequest<VenueReviewsResponseDto>;
