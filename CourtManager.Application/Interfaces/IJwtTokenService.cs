using CourtManager.Domain.Entities;
using System.Security.Claims;

namespace CourtManager.Application.Interfaces;

/// <summary>
/// Contract for generating and validating authentication tokens.
/// </summary>
public interface IJwtTokenService
{
    /// <summary>
    /// Generates an access token for the specified user.
    /// </summary>
    string GenerateAccessToken(User user, IList<string> roles);

    /// <summary>
    /// Generates a refresh token for the specified user.
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
