namespace _116.Shared.Application.Exceptions;

/// <summary>
/// Exception thrown when authorization fails.
/// </summary>
public sealed class AuthorizationException : Exception
{
    /// <summary>
    /// The permission or role required for access.
    /// </summary>
    public string RequiredPermission { get; }

    public AuthorizationException(string message, string requiredPermission) : base(message)
    {
        RequiredPermission = requiredPermission;
    }
}
