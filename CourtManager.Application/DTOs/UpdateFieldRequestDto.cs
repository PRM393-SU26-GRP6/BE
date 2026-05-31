using CourtManager.Domain.Enums;

namespace CourtManager.Application.DTOs;

/// <summary>
/// DTO for updating a football field's name, type, and price.
/// </summary>
public class UpdateFieldRequestDto
{
    public string FieldName { get; set; } = string.Empty;
    public FieldType FieldType { get; set; }
    public decimal PricePerHour { get; set; }
}
