using Microsoft.AspNetCore.Identity;

namespace CourtManager.Domain.Entities;

/// <summary>
/// Join entity for User and Role (many-to-many relationship).
/// </summary>
public class UserRole : IdentityUserRole<Guid>
{
    public DateTime AssignedAt { get; set; }

    // Navigation properties
    public virtual User User { get; set; } = null!;
    public virtual Role Role { get; set; } = null!;
}
