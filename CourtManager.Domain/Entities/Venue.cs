namespace CourtManager.Domain.Entities;

/// <summary>
/// Represents a venue (a complex of football fields) owned by a user.
/// </summary>
public class Venue
{
    public Guid VenueId { get; set; }
    public Guid OwnerId { get; set; }
    public string VenueName { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
    public string Description { get; set; } = string.Empty;
    public string OpeningHours { get; set; } = string.Empty;
    public string PhoneContact { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; }

    // Navigation properties
    public User? Owner { get; set; }
    public ICollection<FootballField> FootballFields { get; set; } = [];
    public ICollection<VenueImage> VenueImages { get; set; } = [];
    public ICollection<VenueAmenity> VenueAmenities { get; set; } = [];
    public ICollection<Review> Reviews { get; set; } = [];
    public ICollection<Discount> Discounts { get; set; } = [];
}
