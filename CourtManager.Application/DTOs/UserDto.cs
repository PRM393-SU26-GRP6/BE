namespace CourtManager.Application.DTOs;

/// <summary>
/// Data Transfer Object for User.
/// </summary>
public class UserDto
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string? AvatarUrl { get; set; }
    public int LoyaltyPoints { get; set; }
    public bool IsActive { get; set; }
    public IEnumerable<string> Roles { get; set; } = [];
}

public class UpdateUserProfileDto
{
    public string FullName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string? AvatarUrl { get; set; }
}
