namespace CourtManager.Application.DTOs;

/// <summary>
/// Data Transfer Object for FootballField.
/// </summary>
public class FootballFieldDto
{
    public Guid Id { get; set; }
    public Guid OwnerId { get; set; }
    public string FieldName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string FieldType { get; set; } = string.Empty;
    public string? VenueAddress { get; set; }
    public decimal PricePerHour { get; set; }
    public bool IsActive { get; set; }
}
