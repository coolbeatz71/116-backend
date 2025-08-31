using System.Security.Claims;
using System.Security.Principal;
using _116.Shared.Application.ErrorHandling.Enums;
using _116.Shared.Application.ErrorHandling.Mappers.Contracts;
using _116.Shared.Application.ErrorHandling.Models;
using _116.Shared.Application.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace _116.Shared.Application.ErrorHandling.Mappers;

/// <summary>
/// Maps authentication and authorization errors to structured error responses.
/// </summary>
public sealed class AuthenticationErrorMapper(ILogger<AuthenticationErrorMapper> logger)
    : BaseErrorMapper(logger), IErrorMapper<Exception>
{
    private static readonly Dictionary<Type, (ErrorTypes ErrorType, int StatusCode)> AuthenticationMappings = new()
    {
        { typeof(UnauthorizedAccessException), (ErrorTypes.AuthenticationFailed, StatusCodes.Status401Unauthorized) },
        { typeof(AuthenticationException), (ErrorTypes.AuthenticationFailed, StatusCodes.Status401Unauthorized) },
        { typeof(AuthorizationException), (ErrorTypes.AccessDenied, StatusCodes.Status403Forbidden) },
        { typeof(JwtTokenException), (ErrorTypes.AuthenticationFailed, StatusCodes.Status401Unauthorized) }
    };

    public override int Priority => 10;

    public override bool CanHandle(object error) => AuthenticationMappings.ContainsKey(error.GetType());

    public override bool CanHandle(Type errorType) => AuthenticationMappings.ContainsKey(errorType);

    public ErrorResponse MapToErrorResponse(Exception exception, HttpContext context)
    {
        return MapToErrorResponse((object)exception, context);
    }

    protected override ErrorResponse CreateErrorResponse(object error, HttpContext context)
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
        IIdentity? identity = context.User.Identity;
        Logger.LogWarning(exception,
            "Unauthorized access attempt for {RequestPath}. User: {User}. Exception: {ExceptionMessage}",
            context.Request.Path,
            identity?.Name ?? "Anonymous",
            exception.Message
        );

        var extensions = new Dictionary<string, object>
        {
            ["authenticationScheme"] = identity?.AuthenticationType ?? "Unknown",
            ["requiresAuthentication"] = identity?.IsAuthenticated != true,
            ["exceptionType"] = exception.GetType().Name
        };

        return ErrorResponse.CreateWithExtensions(
            title: ErrorTypes.AuthenticationFailed.ToString(),
            status: StatusCodes.Status401Unauthorized,
            detail: !string.IsNullOrEmpty(exception.Message)
                ? exception.Message
                : "Authentication required. Please provide valid credentials.",
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
        Logger.LogWarning(
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
            title: ErrorTypes.AuthenticationFailed.ToString(),
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
        IIdentity? identity = context.User.Identity;
        Logger.LogWarning(
            "Authorization failed for user {User} accessing {RequestPath}. Required: {RequiredPermission}",
            identity?.Name ?? "Anonymous",
            context.Request.Path,
            exception.RequiredPermission
        );

        string[] userRoles = context.User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToArray();
        var extensions = new Dictionary<string, object>
        {
            ["requiredPermission"] = exception.RequiredPermission,
            ["userRoles"] = userRoles
        };

        return ErrorResponse.CreateWithExtensions(
            title: ErrorTypes.AccessDenied.ToString(),
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
        Logger.LogWarning(
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
            title: nameof(ErrorTypes.AuthenticationFailed),
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
        Logger.LogWarning(
            "Generic authentication error for {RequestPath}. ErrorType: {ErrorType}",
            context.Request.Path,
            error.GetType().Name
        );

        return ErrorResponse.Create(
            title: nameof(ErrorTypes.AuthenticationFailed),
            status: StatusCodes.Status401Unauthorized,
            detail: "An authentication error occurred while processing your request.",
            instance: context.Request.Path,
            traceId: context.TraceIdentifier
        );
    }
}
