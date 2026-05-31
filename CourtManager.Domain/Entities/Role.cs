using Microsoft.AspNetCore.Identity;

namespace CourtManager.Domain.Entities;

/// <summary>
/// Represents a role in the system (e.g., Admin, User, Owner).
/// </summary>
public class Role : IdentityRole<Guid>
{
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }

    // Navigation property
    public virtual ICollection<UserRole> UserRoles { get; set; } = [];
}
