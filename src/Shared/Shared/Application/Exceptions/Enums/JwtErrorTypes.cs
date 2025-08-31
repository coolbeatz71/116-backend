namespace _116.Shared.Application.Exceptions.Enums;

/// <summary>
/// Enumeration of possible JWT token issues.
/// </summary>
public enum JwtErrorTypes
{
    Missing,
    Invalid,
    Expired,
    Malformed
}
