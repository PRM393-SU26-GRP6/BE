using CourtManager.Application.DTOs;
using CourtManager.Domain.Interfaces;
using MediatR;

namespace CourtManager.Application.Features.Reviews.Queries;

public class GetBookingReviewQueryHandler : IRequestHandler<GetBookingReviewQuery, ReviewDto?>
{
    private readonly IReviewRepository _reviewRepository;

    public GetBookingReviewQueryHandler(IReviewRepository reviewRepository)
    {
        _reviewRepository = reviewRepository;
    }

    public async Task<ReviewDto?> Handle(GetBookingReviewQuery request, CancellationToken cancellationToken)
    {
        var r = await _reviewRepository.GetReviewByBookingIdAsync(request.BookingId, cancellationToken);
        
        if (r == null) return null;

        return new ReviewDto
        {
            ReviewId = r.ReviewId,
            UserId = r.UserId,
            UserName = r.User?.FullName ?? "Anonymous",
            VenueId = r.VenueId,
            BookingId = r.BookingId,
            Rating = r.Rating,
            Comment = r.Comment,
            CreatedAt = r.CreatedAt
        };
    }
}
