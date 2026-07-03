using MediatR;

namespace CourtManager.Application.Features.Reviews.Commands;

/// <summary>
/// Command to delete a review (soft delete).
/// </summary>
public class DeleteReviewCommand : IRequest<bool>
{
    /// <summary>
    /// The ID of the review to delete.
    /// </summary>
    public Guid ReviewId { get; set; }
    public Guid UserId { get; set; }
    public bool IsAdminOrOwner { get; set; }

    public DeleteReviewCommand(Guid reviewId, Guid userId, bool isAdminOrOwner)
    {
        ReviewId = reviewId;
        UserId = userId;
        IsAdminOrOwner = isAdminOrOwner;
    }
}
