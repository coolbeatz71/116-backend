using _116.Core.Application.ErrorHandling.Enums;

namespace _116.Core.Application.Exceptions;

/// <summary>
/// Exception thrown when JWT token issues occur.
/// </summary>
public sealed class JwtTokenException : Exception
{
    /// <summary>
    /// The specific issue with the JWT token.
    /// </summary>
    public JwtTokenIssue Issue { get; }

    /// <summary>
    /// Whether the token is expired.
    /// </summary>
    public bool IsExpired { get; }

    public JwtTokenException(string message, JwtTokenIssue issue, bool isExpired = false) : base(message)
    {
        Issue = issue;
        IsExpired = isExpired;
    }
}
