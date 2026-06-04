using System.Security.Claims;
using CourtManager.Domain.Entities;

namespace CourtManager.Application.Interfaces;

/// <summary>
/// Service contract for generating and validating JWT tokens.
/// </summary>
public interface IJwtTokenService
{
    /// <summary>
    /// Generates an access token for the specified user.
    /// </summary>
    string GenerateAccessToken(User user, IList<string> roles);

    /// <summary>
    /// Generates a JWT refresh token for the specified user.
    /// </summary>
    string GenerateRefreshTokenJwt(User user, IList<string> roles);

    /// <summary>
    /// Gets the expiry time for a refresh token based on configuration.
    /// </summary>
    DateTime GetRefreshTokenExpiryTime();

    /// <summary>
    /// Gets the principal from an expired access token.
    /// </summary>
    ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
}
