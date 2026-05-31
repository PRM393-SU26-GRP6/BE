namespace CourtManager.Application.DTOs;

public class VenueReviewsResponseDto
{
    public IEnumerable<ReviewDto> Reviews { get; set; } = [];
    public int TotalCount { get; set; }
    public decimal AverageRating { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
}
