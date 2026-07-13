namespace CourtManager.Application.DTOs;

public class VenueDetailDto : VenueDto
{
    public List<AmenityDto> Amenities { get; set; } = new();
    public List<FootballFieldDto> Fields { get; set; } = new();
}
