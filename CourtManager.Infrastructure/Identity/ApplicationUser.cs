using Microsoft.AspNetCore.Identity;

namespace CourtManager.Infrastructure.Identity;

/// <summary>
/// ASP.NET Identity user entity — used exclusively for authentication concerns
/// (password hashing, token generation, lockout, etc.).
/// Business user data lives in <see cref="CourtManager.Domain.Entities.User"/>.
/// Both entities share the same <see cref="Guid"/> Id.
/// </summary>
public class ApplicationUser : IdentityUser<Guid>
{
    // No business fields here — they live in Domain User.
}
