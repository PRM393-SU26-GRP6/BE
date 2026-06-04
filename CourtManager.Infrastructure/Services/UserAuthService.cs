using CourtManager.Application.Interfaces;
using CourtManager.Domain.Entities;
using CourtManager.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CourtManager.Infrastructure.Services;

/// <summary>
/// Infrastructure implementation of IUserAuthService.
/// Uses ApplicationUser + UserManager for authentication concerns (password, tokens).
/// Uses Domain User + DbContext for business data and role management.
/// Domain User and ApplicationUser share the same Guid Id.
/// </summary>
public class UserAuthService : IUserAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _dbContext;

    public UserAuthService(UserManager<ApplicationUser> userManager, ApplicationDbContext dbContext)
    {
        _userManager = userManager;
        _dbContext = dbContext;
    }

    /// <inheritdoc/>
    public async Task<User?> FindByEmailAsync(string email)
        => await _dbContext.DomainUsers
            .FirstOrDefaultAsync(u => u.Email == email);

    /// <inheritdoc/>
    public async Task<User?> FindByIdAsync(Guid userId)
        => await _dbContext.DomainUsers
            .FirstOrDefaultAsync(u => u.Id == userId);

    /// <inheritdoc/>
    public async Task<bool> CheckPasswordAsync(User user, string password)
    {
        var appUser = await _userManager.FindByIdAsync(user.Id.ToString());
        return appUser != null && await _userManager.CheckPasswordAsync(appUser, password);
    }

    /// <inheritdoc/>
    public async Task<(bool Succeeded, IEnumerable<string> Errors)> ChangePasswordAsync(
        User user, string currentPassword, string newPassword)
    {
        var appUser = await _userManager.FindByIdAsync(user.Id.ToString());
        if (appUser == null)
            return (false, new[] { "User not found in Identity store." });

        var result = await _userManager.ChangePasswordAsync(appUser, currentPassword, newPassword);
        return (result.Succeeded, result.Errors.Select(e => e.Description));
    }

    /// <inheritdoc/>
    public async Task<(bool Succeeded, IEnumerable<string> Errors)> CreateAsync(User user, string password)
    {
        // Assign new Id if not set
        if (user.Id == Guid.Empty)
            user.Id = Guid.NewGuid();

        // Create Identity (AspNetUsers) record
        var appUser = new ApplicationUser
        {
            Id = user.Id,
            UserName = user.UserName.Length > 0 ? user.UserName : user.Email,
            NormalizedUserName = user.Email.ToUpperInvariant(),
            Email = user.Email,
            NormalizedEmail = user.Email.ToUpperInvariant(),
            PhoneNumber = user.PhoneNumber ?? user.Phone,
            EmailConfirmed = true,
            SecurityStamp = Guid.NewGuid().ToString(),
            ConcurrencyStamp = Guid.NewGuid().ToString()
        };

        var result = await _userManager.CreateAsync(appUser, password);
        if (!result.Succeeded)
            return (false, result.Errors.Select(e => e.Description));

        // Create Domain (Users) record
        _dbContext.DomainUsers.Add(user);
        await _dbContext.SaveChangesAsync();

        return (true, Enumerable.Empty<string>());
    }

    /// <inheritdoc/>
    public async Task<(bool Succeeded, IEnumerable<string> Errors)> UpdateAsync(User user)
    {
        // Update Domain User
        _dbContext.DomainUsers.Update(user);

        // Sync email / phone to ApplicationUser
        var appUser = await _userManager.FindByIdAsync(user.Id.ToString());
        if (appUser != null)
        {
            appUser.Email = user.Email;
            appUser.NormalizedEmail = user.Email.ToUpperInvariant();
            appUser.PhoneNumber = user.PhoneNumber ?? user.Phone;
            await _userManager.UpdateAsync(appUser);
        }

        await _dbContext.SaveChangesAsync();
        return (true, Enumerable.Empty<string>());
    }

    /// <inheritdoc/>
    public async Task<IList<string>> GetRolesAsync(User user)
        => await _dbContext.DomainUserRoles
            .Where(ur => ur.UserId == user.Id)
            .Select(ur => ur.Role.Name)
            .ToListAsync();

    /// <inheritdoc/>
    public async Task<(bool Succeeded, IEnumerable<string> Errors)> AddToRoleAsync(User user, string roleName)
    {
        var role = await _dbContext.DomainRoles
            .FirstOrDefaultAsync(r => r.Name == roleName);
        if (role == null)
            return (false, new[] { $"Role '{roleName}' not found." });

        var exists = await _dbContext.DomainUserRoles
            .AnyAsync(ur => ur.UserId == user.Id && ur.RoleId == role.Id);
        if (!exists)
        {
            _dbContext.DomainUserRoles.Add(new UserRole
            {
                UserId = user.Id,
                RoleId = role.Id,
                AssignedAt = DateTime.UtcNow
            });
            await _dbContext.SaveChangesAsync();
        }

        return (true, Enumerable.Empty<string>());
    }

    /// <inheritdoc/>
    public async Task<(bool Succeeded, IEnumerable<string> Errors)> RemoveFromRolesAsync(
        User user, IEnumerable<string> roleNames)
    {
        var names = roleNames.ToList();
        var toRemove = await _dbContext.DomainUserRoles
            .Where(ur => ur.UserId == user.Id && names.Contains(ur.Role.Name))
            .ToListAsync();

        _dbContext.DomainUserRoles.RemoveRange(toRemove);
        await _dbContext.SaveChangesAsync();

        return (true, Enumerable.Empty<string>());
    }

    /// <inheritdoc/>
    public async Task<string> GeneratePasswordResetTokenAsync(User user)
    {
        var appUser = await _userManager.FindByIdAsync(user.Id.ToString());
        if (appUser == null)
            throw new InvalidOperationException($"ApplicationUser not found for Id {user.Id}.");

        return await _userManager.GeneratePasswordResetTokenAsync(appUser);
    }

    /// <inheritdoc/>
    public async Task<(bool Succeeded, IEnumerable<string> Errors)> ResetPasswordAsync(
        User user, string token, string newPassword)
    {
        var appUser = await _userManager.FindByIdAsync(user.Id.ToString());
        if (appUser == null)
            return (false, new[] { "User not found in Identity store." });

        var result = await _userManager.ResetPasswordAsync(appUser, token, newPassword);
        return (result.Succeeded, result.Errors.Select(e => e.Description));
    }

    /// <inheritdoc/>
    public IQueryable<User> Users => _dbContext.DomainUsers;
}
