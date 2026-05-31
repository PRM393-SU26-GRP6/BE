namespace CourtManager.Application.DTOs;

/// <summary>
/// DTO for toggling a football field's active status.
/// </summary>
public class UpdateFieldStatusRequestDto
{
    public bool IsActive { get; set; }
}
