namespace _116.Shared.Application.ErrorHandling.Enums;

/// <summary>
/// Enumeration of possible JWT token issues.
/// </summary>
public enum JwtTokenIssue
{
    Missing,
    Invalid,
    Expired,
    Malformed,
    Unauthorized
}
