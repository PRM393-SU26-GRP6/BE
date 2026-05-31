namespace CourtManager.Application.DTOs;

/// <summary>
/// DTO for updating the current user's profile.
/// Email and password are NOT updated here.
/// </summary>
public class UpdateProfileRequestDto
{
    public string FullName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? AvatarUrl { get; set; }
}
