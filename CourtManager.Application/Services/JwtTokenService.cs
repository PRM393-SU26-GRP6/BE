using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using CourtManager.Domain.Entities;

namespace CourtManager.Application.Services;

/// <summary>
/// Service for generating and validating JWT tokens.
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

public class JwtTokenService : IJwtTokenService
{
    private readonly string _secret;
    private readonly string _issuer;
    private readonly string _audience;
    private readonly int _accessTokenExpirationInMinutes;
    private readonly int _refreshTokenExpirationInDays;

    public JwtTokenService(
        string secret,
        string issuer,
        string audience,
        int accessTokenExpirationInMinutes = 60,
        int refreshTokenExpirationInDays = 7)
    {
        _secret = secret;
        _issuer = issuer;
        _audience = audience;
        _accessTokenExpirationInMinutes = accessTokenExpirationInMinutes;
        _refreshTokenExpirationInDays = refreshTokenExpirationInDays;
    }

    /// <summary>
    /// Generates a JWT access token for the specified user.
    /// </summary>
    public string GenerateAccessToken(User user, IList<string> roles)
    {
        var securityKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_secret));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
            new Claim(ClaimTypes.Name, user.FullName ?? string.Empty),
            new Claim("PhoneNumber", user.PhoneNumber ?? string.Empty)
        };

        // Add roles as claims
        if (roles != null)
        {
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
        }

        var token = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_accessTokenExpirationInMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    /// <summary>
    /// Generates a JWT refresh token for the specified user.
    /// </summary>
    public string GenerateRefreshTokenJwt(User user, IList<string> roles)
    {
        var securityKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_secret));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
            new Claim(ClaimTypes.Name, user.FullName ?? string.Empty),
            new Claim("token_type", "refresh")
        };

        if (roles != null)
        {
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
        }

        var token = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            expires: DateTime.UtcNow.AddDays(_refreshTokenExpirationInDays),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    /// <summary>
    /// Gets the expiry time for a refresh token.
    /// </summary>
    public DateTime GetRefreshTokenExpiryTime()
    {
        return DateTime.UtcNow.AddDays(_refreshTokenExpirationInDays);
    }

    /// <summary>
    /// Extracts the principal from an expired access token for refresh token validation.
    /// </summary>
    public ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
    {
        try
        {
            var securityKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_secret));

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = false, // We want to read an expired token
                ValidateIssuerSigningKey = true,
                ValidIssuer = _issuer,
                ValidAudience = _audience,
                IssuerSigningKey = securityKey,
                ClockSkew = TimeSpan.Zero
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);

            if (!(securityToken is JwtSecurityToken jwtSecurityToken) ||
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                return null;
            }

            return principal;
        }
        catch
        {
            return null;
        }
    }
}
