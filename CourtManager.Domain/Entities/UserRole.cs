namespace CourtManager.Domain.Entities;

/// <summary>
/// Join entity for User and Role (many-to-many relationship).
/// Pure domain POCO — no framework inheritance.
/// </summary>
public class UserRole
{
    public Guid UserId { get; set; }
    public Guid RoleId { get; set; }
    public DateTime AssignedAt { get; set; }

    // Navigation properties
    public virtual User User { get; set; } = null!;
    public virtual Role Role { get; set; } = null!;
}
