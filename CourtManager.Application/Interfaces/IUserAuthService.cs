using CourtManager.Domain.Entities;

namespace CourtManager.Application.Interfaces;

/// <summary>
/// Service contract for user authentication and management operations.
/// Abstracts ASP.NET Identity's UserManager away from Application layer handlers.
/// </summary>
public interface IUserAuthService
{
    /// <summary>Finds a user by email address.</summary>
    Task<User?> FindByEmailAsync(string email);

    /// <summary>Finds a user by their unique identifier.</summary>
    Task<User?> FindByIdAsync(Guid userId);

    /// <summary>Checks if the given password is valid for the user.</summary>
    Task<bool> CheckPasswordAsync(User user, string password);

    /// <summary>Changes the user's password after verifying the current one.</summary>
    Task<(bool Succeeded, IEnumerable<string> Errors)> ChangePasswordAsync(User user, string currentPassword, string newPassword);

    /// <summary>Creates a new user with the specified password.</summary>
    Task<(bool Succeeded, IEnumerable<string> Errors)> CreateAsync(User user, string password);

    /// <summary>Updates an existing user's data.</summary>
    Task<(bool Succeeded, IEnumerable<string> Errors)> UpdateAsync(User user);

    /// <summary>Returns the list of role names assigned to the user.</summary>
    Task<IList<string>> GetRolesAsync(User user);

    /// <summary>Adds the user to a role.</summary>
    Task<(bool Succeeded, IEnumerable<string> Errors)> AddToRoleAsync(User user, string role);

    /// <summary>Removes the user from a set of roles.</summary>
    Task<(bool Succeeded, IEnumerable<string> Errors)> RemoveFromRolesAsync(User user, IEnumerable<string> roles);

    /// <summary>Generates a password reset token for the user.</summary>
    Task<string> GeneratePasswordResetTokenAsync(User user);

    /// <summary>Resets the user's password using the provided token.</summary>
    Task<(bool Succeeded, IEnumerable<string> Errors)> ResetPasswordAsync(User user, string token, string newPassword);

    /// <summary>Returns a queryable collection of all users (for filtering/projection).</summary>
    IQueryable<User> Users { get; }
}
