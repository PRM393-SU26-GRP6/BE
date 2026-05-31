using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CourtManager.APIs.Controllers;

/// <summary>
/// Base controller that provides CurrentUserId helper.
/// All controllers that need the logged-in user's ID should inherit from this.
/// </summary>
[ApiController]
public abstract class BaseApiController : ControllerBase
{
    /// <summary>
    /// Returns the current user's Guid from the JWT claim.
    /// Throws UnauthorizedException (401) via global middleware if the claim is missing or invalid.
    /// </summary>
    protected Guid CurrentUserId
    {
        get
        {
            var raw = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(raw, out var id))
            {
                throw new UnauthorizedAccessException("Invalid or missing user identity claim.");
            }
            return id;
        }
    }
}
