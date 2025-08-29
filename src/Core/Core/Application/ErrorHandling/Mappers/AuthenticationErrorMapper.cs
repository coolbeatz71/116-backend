using _116.Core.Application.ErrorHandling.Abstractions;
using _116.Core.Application.ErrorHandling.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace _116.Core.Application.ErrorHandling.Mappers;

/// <summary>
/// Maps authentication and authorization errors to structured error responses.
/// </summary>
/// <remarks>
/// This mapper handles JWT authentication failures, authorization denials,
/// and other authentication-related errors with specialized error responses
/// that provide appropriate context for client applications.
/// </remarks>
public sealed class AuthenticationErrorMapper(ILogger<AuthenticationErrorMapper> logger) : IErrorMapper
{
    /// <summary>
    /// Gets the priority of this mapper (higher than generic exception mapper).
    /// </summary>
    public int Priority => 10;

    /// <summary>
    /// Determines if this mapper can handle the specified error.
    /// </summary>
    /// <param name="error">The error to evaluate</param>
    /// <returns>True if this mapper can handle authentication-related errors</returns>
    public bool CanHandle(object error)
    {
        return error switch
        {
            UnauthorizedAccessException => true,
            AuthenticationException => true,
            AuthorizationException => true,
            JwtTokenException => true,
            _ => false
        };
    }

    /// <summary>
    /// Maps authentication errors to standardized error responses.
    /// </summary>
    /// <param name="error">The authentication error to map</param>
    /// <param name="context">The current HTTP context</param>
    /// <returns>A structured error response</returns>
    public ErrorResponse MapToErrorResponse(object error, HttpContext context)
    {
        return error switch
        {
            UnauthorizedAccessException unauthorizedEx => CreateUnauthorizedResponse(unauthorizedEx, context),
            AuthenticationException authenticationEx => CreateAuthenticationResponse(authenticationEx, context),
            AuthorizationException authorizationEx => CreateAuthorizationResponse(authorizationEx, context),
            JwtTokenException jwtEx => CreateJwtTokenResponse(jwtEx, context),
            _ => CreateGenericAuthenticationResponse(error, context)
        };
    }

    /// <summary>
    /// Creates an error response for unauthorized access.
    /// </summary>
    private ErrorResponse CreateUnauthorizedResponse(UnauthorizedAccessException exception, HttpContext context)
    {
        logger.LogWarning(
            "Unauthorized access attempt for {RequestPath}. User: {User}",
            context.Request.Path,
            context.User.Identity?.Name ?? "Anonymous"
        );

        var extensions = new Dictionary<string, object>
        {
            ["authenticationScheme"] = context.User?.Identity!.AuthenticationType ?? "Unknown",
            ["requiresAuthentication"] = !context.User?.Identity!.IsAuthenticated ?? true
        };

        return ErrorResponse.CreateWithExtensions(
            title: "Unauthorized",
            status: StatusCodes.Status401Unauthorized,
            detail: "Authentication required. Please provide valid credentials.",
            instance: context.Request.Path,
            traceId: context.TraceIdentifier,
            extensions: extensions
        );
    }

    /// <summary>
    /// Creates an error response for authentication failures.
    /// </summary>
    private ErrorResponse CreateAuthenticationResponse(AuthenticationException exception, HttpContext context)
    {
        logger.LogWarning(
            "Authentication failed for {RequestPath}. Reason: {Reason}",
            context.Request.Path,
            exception.Message
        );

        var extensions = new Dictionary<string, object>
        {
            ["reason"] = exception.Reason,
            ["authenticationScheme"] = exception.AuthenticationScheme ?? "Unknown"
        };

        return ErrorResponse.CreateWithExtensions(
            title: "Authentication Failed",
            status: StatusCodes.Status401Unauthorized,
            detail: exception.Message,
            instance: context.Request.Path,
            traceId: context.TraceIdentifier,
            extensions: extensions
        );
    }

    /// <summary>
    /// Creates an error response for authorization failures.
    /// </summary>
    private ErrorResponse CreateAuthorizationResponse(AuthorizationException exception, HttpContext context)
    {
        logger.LogWarning(
            "Authorization failed for user {User} accessing {RequestPath}. Required: {RequiredPermission}",
            context.User?.Identity?.Name ?? "Anonymous",
            context.Request.Path,
            exception.RequiredPermission
        );

        var extensions = new Dictionary<string, object>
        {
            ["requiredPermission"] = exception.RequiredPermission,
            ["userRoles"] = context.User?.FindAll("role")?.Select(c => c.Value).ToArray() ?? []
        };

        return ErrorResponse.CreateWithExtensions(
            title: "Forbidden",
            status: StatusCodes.Status403Forbidden,
            detail: exception.Message,
            instance: context.Request.Path,
            traceId: context.TraceIdentifier,
            extensions: extensions
        );
    }

    /// <summary>
    /// Creates an error response for JWT token errors.
    /// </summary>
    private ErrorResponse CreateJwtTokenResponse(JwtTokenException exception, HttpContext context)
    {
        logger.LogWarning(
            "JWT token error for {RequestPath}. Issue: {Issue}",
            context.Request.Path,
            exception.Issue
        );

        var extensions = new Dictionary<string, object>
        {
            ["tokenIssue"] = exception.Issue,
            ["isExpired"] = exception.IsExpired,
            ["tokenType"] = "Bearer"
        };

        int statusCode = exception.Issue switch
        {
            JwtTokenIssue.Expired => StatusCodes.Status401Unauthorized,
            JwtTokenIssue.Invalid => StatusCodes.Status401Unauthorized,
            JwtTokenIssue.Missing => StatusCodes.Status401Unauthorized,
            JwtTokenIssue.Malformed => StatusCodes.Status400BadRequest,
            _ => StatusCodes.Status401Unauthorized
        };

        return ErrorResponse.CreateWithExtensions(
            title: "Token Error",
            status: statusCode,
            detail: exception.Message,
            instance: context.Request.Path,
            traceId: context.TraceIdentifier,
            extensions: extensions
        );
    }

    /// <summary>
    /// Creates a generic authentication error response.
    /// </summary>
    private ErrorResponse CreateGenericAuthenticationResponse(object error, HttpContext context)
    {
        logger.LogWarning(
            "Generic authentication error for {RequestPath}. ErrorType: {ErrorType}",
            context.Request.Path,
            error.GetType().Name
        );

        return ErrorResponse.Create(
            title: "Authentication Error",
            status: StatusCodes.Status401Unauthorized,
            detail: "An authentication error occurred while processing your request.",
            instance: context.Request.Path,
            traceId: context.TraceIdentifier
        );
    }
}

#region Custom Authentication Exception Types

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

#endregion
