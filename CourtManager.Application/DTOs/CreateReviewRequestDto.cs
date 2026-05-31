namespace CourtManager.Application.DTOs;

public class CreateReviewRequestDto
{
    public Guid VenueId { get; set; }
    public Guid BookingId { get; set; }
    public int Rating { get; set; }
    public string? Comment { get; set; }
}
