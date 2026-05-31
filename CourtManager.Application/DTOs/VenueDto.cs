namespace CourtManager.Application.DTOs;

public class VenueDto
{
    public Guid VenueId { get; set; }
    public string VenueName { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
    public double? Distance { get; set; } // For nearby queries
    public string Description { get; set; } = string.Empty;
    public string OpeningHours { get; set; } = string.Empty;
    public string PhoneContact { get; set; } = string.Empty;
    
    // Aggregate properties calculated dynamically
    public string OwnerName { get; set; } = string.Empty;
    public double AverageRating { get; set; }
    public int TotalReviews { get; set; }
    public decimal MinPrice { get; set; }
    public decimal MaxPrice { get; set; }
}
