namespace _116.Shared.Application.Exceptions.Enums;

/// <summary>
/// Standard error types
/// </summary>
public enum ErrorTypes
{
    ValidationFailed,
    BadRequest,
    NotFound,
    AuthenticationFailed,
    AccessDenied,
    InternalError,
    Conflict,
    Timeout,
    RateLimitExceeded,
    ServiceUnavailable
}
