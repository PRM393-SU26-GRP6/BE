namespace CourtManager.Application.Interfaces;

/// <summary>
/// Service contract for hashing and verifying passwords.
/// </summary>
public interface IPasswordHasherService
{
    /// <summary>
    /// Hashes a password.
    /// </summary>
    string HashPassword(string password);

    /// <summary>
    /// Verifies a password against a hash.
    /// </summary>
    bool VerifyPassword(string password, string hash);
}
