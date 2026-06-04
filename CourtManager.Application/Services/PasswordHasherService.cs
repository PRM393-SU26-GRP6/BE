using BCrypt.Net;

namespace CourtManager.Application.Services;

/// <summary>
/// Service for hashing and verifying passwords using BCrypt.
/// </summary>
public interface IPasswordHasherService
{
    /// <summary>
    /// Hashes a password using BCrypt.
    /// </summary>
    string HashPassword(string password);

    /// <summary>
    /// Verifies a password against a BCrypt hash.
    /// </summary>
    bool VerifyPassword(string password, string hash);
}

public class PasswordHasherService : IPasswordHasherService
{
    /// <summary>
    /// Hashes a password using BCrypt.
    /// </summary>
    public string HashPassword(string password)
    {
        return global::BCrypt.Net.BCrypt.HashPassword(password);
    }

    /// <summary>
    /// Verifies a password against a BCrypt hash.
    /// </summary>
    public bool VerifyPassword(string password, string hash)
    {
        try
        {
            return global::BCrypt.Net.BCrypt.Verify(password, hash);
        }
        catch
        {
            return false;
        }
    }
}
