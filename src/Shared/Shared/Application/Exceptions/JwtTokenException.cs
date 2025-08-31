using _116.Shared.Application.Exceptions.Enums;

namespace _116.Shared.Application.Exceptions;

/// <summary>
/// Exception thrown when JWT token issues occur.
/// </summary>
public sealed class JwtTokenException(
    string message,
    JwtErrorTypes issue,
    bool isExpired = false
) : Exception(message)
{
    /// <summary>
    /// The specific issue with the JWT token.
    /// </summary>
    public JwtErrorTypes Issue { get; } = issue;

    /// <summary>
    /// Whether the token is expired.
    /// </summary>
    public bool IsExpired { get; } = isExpired;
}
