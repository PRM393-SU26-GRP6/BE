namespace CourtManager.Application.DTOs;

/// <summary>
/// DTO for updating an existing venue's information.
/// </summary>
public class UpdateVenueRequestDto
{
    public string VenueName { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
    public string Description { get; set; } = string.Empty;
    public string OpeningHours { get; set; } = string.Empty;
    public string PhoneContact { get; set; } = string.Empty;
}
