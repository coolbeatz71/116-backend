namespace _116.Shared.Application.ErrorHandling.Enums;

/// <summary>
/// Standard error types for consistent error identification across frontend and backend.
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
