using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CourtManager.APIs.Attributes;

namespace CourtManager.APIs.Controllers;

/// <summary>
/// Controller for admin operations requiring specific roles.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AdminController : ControllerBase
{
    /// <summary>
    /// Get user statistics (Admin only).
    /// </summary>
    [HttpGet("stats")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public IActionResult GetStatistics()
    {
        return Ok(new
        {
            message = "Admin statistics",
            totalUsers = 150,
            totalBookings = 500
        });
    }

    /// <summary>
    /// Manage courts (Admin or Manager only).
    /// </summary>
    [HttpGet("courts")]
    [Authorize(Roles = "Admin,Owner")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public IActionResult ManageCourts()
    {
        return Ok(new
        {
            message = "Court management interface",
            courts = new[] { "Court 1", "Court 2" }
        });
    }

    /// <summary>
    /// Example of checking role programmatically.
    /// </summary>
    [HttpPost("test-role")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public IActionResult TestRole()
    {
        var userRole = User.FindFirst("http://schemas.microsoft.com/ws/2008/06/identity/claims/role")?.Value;

        if (userRole == "Admin")
        {
            return Ok(new { message = "You are an Admin!" });
        }

        return Forbid();
    }
}
