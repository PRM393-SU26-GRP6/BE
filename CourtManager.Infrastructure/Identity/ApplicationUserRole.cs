using Microsoft.AspNetCore.Identity;

namespace CourtManager.Infrastructure.Identity;

/// <summary>
/// ASP.NET Identity user-role join entity — used only for Identity role management infrastructure.
/// Business role assignments live in <see cref="CourtManager.Domain.Entities.UserRole"/>.
/// </summary>
public class ApplicationUserRole : IdentityUserRole<Guid>
{
    // No business fields here — they live in Domain UserRole.
}
