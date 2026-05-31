namespace CourtManager.Application.DTOs;

/// <summary>
/// Data Transfer Object for Review entity.
/// Represents a review/rating given by a user for a football field.
/// </summary>
public class ReviewDto
{
    /// <summary>
    /// Unique identifier for the review.
    /// </summary>
    public Guid ReviewId { get; set; }

    /// <summary>
    /// Foreign key referencing the user who wrote the review.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Display name of the reviewer (optional).
    /// </summary>
    public string? UserName { get; set; }

    /// <summary>
    /// Foreign key referencing the reviewed venue.
    /// </summary>
    public Guid VenueId { get; set; }

    /// <summary>
    /// Foreign key referencing the booking.
    /// </summary>
    public Guid BookingId { get; set; }

    /// <summary>
    /// Rating score (typically 1-5).
    /// </summary>
    public int Rating { get; set; }

    /// <summary>
    /// Optional detailed comment/feedback about the field.
    /// </summary>
    public string? Comment { get; set; }

    /// <summary>
    /// Timestamp when the review was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }
}
