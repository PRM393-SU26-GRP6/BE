namespace CourtManager.Application.Exceptions;

/// <summary>
/// Exception thrown when an authenticated user tries to access a resource they do not own / are not allowed to.
/// Maps to HTTP 403 Forbidden.
/// </summary>
public class ForbiddenException : Exception
{
    public ForbiddenException(string message) : base(message) { }
}
