namespace _116.Shared.Application.Exceptions;

/// <summary>
/// Exception thrown when authentication fails.
/// </summary>
public sealed class AuthenticationException : Exception
{
    /// <summary>
    /// The reason for authentication failure.
    /// </summary>
    public string Reason { get; }

    /// <summary>
    /// The authentication scheme that was used.
    /// </summary>
    public string? AuthenticationScheme { get; }

    public AuthenticationException(string message, string reason, string? authenticationScheme = null)
        : base(message)
    {
        Reason = reason;
        AuthenticationScheme = authenticationScheme;
    }
}
