using CourtManager.Application.DTOs;
using CourtManager.Application.Exceptions;
using CourtManager.Domain.Entities;
using CourtManager.Domain.Interfaces;
using MediatR;

namespace CourtManager.Application.Features.Reviews.Commands;

public class CreateReviewCommandHandler : IRequestHandler<CreateReviewCommand, ReviewDto>
{
    private readonly IReviewRepository _reviewRepository;
    private readonly IBookingRepository _bookingRepository;

    public CreateReviewCommandHandler(IReviewRepository reviewRepository, IBookingRepository bookingRepository)
    {
        _reviewRepository = reviewRepository;
        _bookingRepository = bookingRepository;
    }

    public async Task<ReviewDto> Handle(CreateReviewCommand request, CancellationToken cancellationToken)
    {
        // 1. Verify that the booking is valid for review (belongs to user, belongs to venue, and is completed)
        bool isValid = await _bookingRepository.IsBookingValidForReviewAsync(request.BookingId, request.VenueId, request.UserId, cancellationToken);
        if (!isValid)
        {
            throw new InvalidOperationException("Cannot create review. Booking must be completed, belong to you, and belong to the specified venue.");
        }

        // 2. Verify that this booking has not been reviewed yet
        var existingReview = await _reviewRepository.GetReviewByBookingIdAsync(request.BookingId, cancellationToken);
        if (existingReview != null)
        {
            throw new InvalidOperationException("You have already reviewed this booking.");
        }

        // 3. Create the review
        var review = new Review
        {
            ReviewId = Guid.NewGuid(),
            VenueId = request.VenueId,
            BookingId = request.BookingId,
            UserId = request.UserId,
            Rating = request.Rating,
            Comment = request.Comment,
            CreatedAt = DateTime.UtcNow
        };

        await _reviewRepository.AddAsync(review, cancellationToken);
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
