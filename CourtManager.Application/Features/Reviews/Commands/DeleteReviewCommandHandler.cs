using MediatR;
using CourtManager.Application.Interfaces;
using CourtManager.Application.Exceptions;
using Microsoft.Extensions.Logging;

namespace CourtManager.Application.Features.Reviews.Commands;

/// <summary>
/// Handler for DeleteReviewCommand.
/// Performs soft delete on review.
/// </summary>
public class DeleteReviewCommandHandler : IRequestHandler<DeleteReviewCommand, bool>
{
    private readonly IReviewRepository _reviewRepository;
    private readonly ILogger<DeleteReviewCommandHandler> _logger;

    public DeleteReviewCommandHandler(IReviewRepository reviewRepository, ILogger<DeleteReviewCommandHandler> logger)
    {
        _reviewRepository = reviewRepository;
        _logger = logger;
    }

    public async Task<bool> Handle(DeleteReviewCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling DeleteReviewCommand for ReviewId: {ReviewId}", request.ReviewId);

        var review = await _reviewRepository.GetByIdAsync(request.ReviewId, cancellationToken);
        if (review == null)
        {
            _logger.LogWarning("Review {ReviewId} not found", request.ReviewId);
            throw new NotFoundException($"Review with ID {request.ReviewId} not found");
        }

        await _reviewRepository.DeleteAsync(review, cancellationToken);
        await _reviewRepository.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Review {ReviewId} deleted successfully (soft delete)", request.ReviewId);
        return true;
    }
}
