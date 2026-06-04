using Microsoft.AspNetCore.Identity;

namespace CourtManager.Infrastructure.Identity;

/// <summary>
/// ASP.NET Identity role entity — used only for Identity role management infrastructure.
/// Business role data lives in <see cref="CourtManager.Domain.Entities.Role"/>.
/// </summary>
public class ApplicationRole : IdentityRole<Guid>
{
    // No business fields here — they live in Domain Role.
}
