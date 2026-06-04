namespace CourtManager.Domain.Entities;

/// <summary>
/// Represents a role in the system (e.g., Admin, User, Owner).
/// Pure domain POCO — no framework inheritance.
/// </summary>
public class Role
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? NormalizedName { get; set; }
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }

    // Navigation property
    public virtual ICollection<UserRole> UserRoles { get; set; } = [];
}
