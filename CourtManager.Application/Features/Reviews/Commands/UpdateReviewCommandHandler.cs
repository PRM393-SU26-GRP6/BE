using CourtManager.Application.DTOs;
using CourtManager.Domain.Interfaces;
using MediatR;

namespace CourtManager.Application.Features.Reviews.Commands;

public class UpdateReviewCommandHandler : IRequestHandler<UpdateReviewCommand, ReviewDto>
{
    private readonly IReviewRepository _reviewRepository;

    public UpdateReviewCommandHandler(IReviewRepository reviewRepository)
    {
        _reviewRepository = reviewRepository;
    }

    public async Task<ReviewDto> Handle(UpdateReviewCommand request, CancellationToken cancellationToken)
    {
        var review = await _reviewRepository.GetByIdAsync(request.ReviewId, cancellationToken);
        
        if (review == null || review.IsDeleted)
        {
            throw new KeyNotFoundException("Review not found.");
        }

        if (review.UserId != request.UserId)
        {
            throw new UnauthorizedAccessException("You can only edit your own reviews.");
        }

        review.Rating = request.Rating;
        review.Comment = request.Comment;
        
        await _reviewRepository.UpdateAsync(review, cancellationToken);
        await _reviewRepository.SaveChangesAsync(cancellationToken);

        return new ReviewDto
        {
            ReviewId = review.ReviewId,
            UserId = review.UserId,
            VenueId = review.VenueId,
            BookingId = review.BookingId,
            Rating = review.Rating,
            Comment = review.Comment,
            CreatedAt = review.CreatedAt
        };
    }
}
