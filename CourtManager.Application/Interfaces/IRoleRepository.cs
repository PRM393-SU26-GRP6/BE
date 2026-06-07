using CourtManager.Domain.Entities;

namespace CourtManager.Application.Interfaces;

/// <summary>
/// Interface for Role repository operations.
/// </summary>
public interface IRoleRepository : IRepository<Role>
{
    /// <summary>
    /// Gets a role by name.
    /// </summary>
    Task<Role?> GetByNameAsync(string name);

    /// <summary>
    /// Adds a role to a user.
    /// </summary>
    Task AddRoleToUserAsync(Guid userId, Guid roleId);

    /// <summary>
    /// Removes a role from a user.
    /// </summary>
    Task RemoveRoleFromUserAsync(Guid userId, Guid roleId);
}
