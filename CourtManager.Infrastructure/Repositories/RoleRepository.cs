using CourtManager.Domain.Entities;
using CourtManager.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CourtManager.Infrastructure.Repositories;

/// <summary>
/// Repository for managing Domain Role and UserRole entities.
/// Uses DomainRoles / DomainUserRoles DbSets (not Identity's AspNetRoles).
/// </summary>
public class RoleRepository : Repository<Role>, IRoleRepository
{
    private readonly ApplicationDbContext _appContext;

    public RoleRepository(ApplicationDbContext context) : base(context)
    {
        _appContext = context;
    }

    /// <summary>
    /// Gets a role by name.
    /// </summary>
    public async Task<Role?> GetByNameAsync(string name)
    {
        return await _appContext.DomainRoles
            .FirstOrDefaultAsync(r => r.Name == name);
    }

    /// <summary>
    /// Adds a role to a user.
    /// </summary>
    public async Task AddRoleToUserAsync(Guid userId, Guid roleId)
    {
        var userRole = new UserRole
        {
            UserId = userId,
            RoleId = roleId,
            AssignedAt = DateTime.UtcNow
        };

        _appContext.DomainUserRoles.Add(userRole);
        await _appContext.SaveChangesAsync();
    }

    /// <summary>
    /// Removes a role from a user.
    /// </summary>
    public async Task RemoveRoleFromUserAsync(Guid userId, Guid roleId)
    {
        var userRole = await _appContext.DomainUserRoles
            .FirstOrDefaultAsync(ur => ur.UserId == userId && ur.RoleId == roleId);

        if (userRole != null)
        {
            _appContext.DomainUserRoles.Remove(userRole);
            await _appContext.SaveChangesAsync();
        }
    }
}
