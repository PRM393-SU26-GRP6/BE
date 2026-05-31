namespace CourtManager.Application.DTOs;

public class VenueImageDto
{
    public Guid ImageId { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public bool IsPrimary { get; set; }
}
