using CourtManager.Application.DTOs;
using CourtManager.Domain.Interfaces;
using MediatR;

namespace CourtManager.Application.Features.Reviews.Queries;

public class GetVenueReviewsQueryHandler : IRequestHandler<GetVenueReviewsQuery, VenueReviewsResponseDto>
{
    private readonly IReviewRepository _reviewRepository;

    public GetVenueReviewsQueryHandler(IReviewRepository reviewRepository)
    {
        _reviewRepository = reviewRepository;
    }

    public async Task<VenueReviewsResponseDto> Handle(GetVenueReviewsQuery request, CancellationToken cancellationToken)
    {
        var result = await _reviewRepository.GetVenueReviewsAsync(request.VenueId, request.Page, request.PageSize, cancellationToken);

        var reviewDtos = result.Reviews.Select(r => new ReviewDto
        {
            ReviewId = r.ReviewId,
            UserId = r.UserId,
            UserName = r.User?.FullName ?? "Anonymous",
            VenueId = r.VenueId,
            BookingId = r.BookingId,
            Rating = r.Rating,
            Comment = r.Comment,
            CreatedAt = r.CreatedAt
        });

        return new VenueReviewsResponseDto
        {
            Reviews = reviewDtos,
            TotalCount = result.TotalCount,
            AverageRating = result.AverageRating,
            Page = request.Page,
            PageSize = request.PageSize
        };
    }
}
