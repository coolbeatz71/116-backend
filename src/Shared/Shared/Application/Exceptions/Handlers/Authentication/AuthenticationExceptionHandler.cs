using System.Security.Principal;
using _116.Shared.Application.Exceptions.Entities;
using _116.Shared.Application.Exceptions.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace _116.Shared.Application.Exceptions.Handlers.Authentication;

/// <summary>
/// Handles generic authentication failures such as <see cref="UnauthorizedAccessException"/>
/// and domain <see cref="AuthenticationException"/>.
/// </summary>
public sealed class AuthenticationExceptionHandler(ILogger<AuthenticationExceptionHandler> logger) : BaseErrorHandler(logger)
{
    /// <inheritdoc />
    public override int Priority => 110;

    /// <inheritdoc />
    public override bool CanHandle(Exception exception) =>
        exception is UnauthorizedAccessException or AuthenticationException;

    /// <inheritdoc />
    protected override ErrorResponseEntity CreateErrorResponse(Exception exception, HttpContext context)
    {
        return exception switch
        {
            UnauthorizedAccessException ex => CreateUnauthorizedResponse(ex, context),
            AuthenticationException ex => CreateAuthenticationResponse(ex, context),
            _ => ErrorResponse(
                ErrorTypes.AuthenticationFailed,
                StatusCodes.Status401Unauthorized,
                "An authentication error occurred while processing your request.",
                context
            )
        };
    }

    /// <summary>
    /// Creates an error response for unauthorized access.
    /// </summary>
    private static ErrorResponseEntity CreateUnauthorizedResponse(
        UnauthorizedAccessException exception,
        HttpContext context
    )
    {
        IIdentity? identity = context.User.Identity;
        var extensions = new Dictionary<string, object>
        {
            ["authenticationScheme"] = identity?.AuthenticationType ?? "Unknown",
            ["requiresAuthentication"] = identity?.IsAuthenticated != true,
            ["exceptionType"] = exception.GetType().Name
        };

        return ErrorResponseEntity.CreateWithExtensions(
            title: nameof(ErrorTypes.AuthenticationFailed),
            status: StatusCodes.Status401Unauthorized,
            detail: string.IsNullOrWhiteSpace(exception.Message)
                ? "Authentication required. Please provide valid credentials."
                : exception.Message,
            instance: context.Request.Path,
            traceId: context.TraceIdentifier,
            extensions: extensions
        );
    }

    /// <summary>
    /// Creates an error response for authentication failures.
    /// </summary>
    private static ErrorResponseEntity CreateAuthenticationResponse(
        AuthenticationException exception,
        HttpContext context
    )
    {
        var extensions = new Dictionary<string, object>
        {
            ["reason"] = exception.Reason,
            ["authenticationScheme"] = exception.AuthenticationScheme ?? "Unknown"
        };

        return ErrorResponseEntity.CreateWithExtensions(
            title: nameof(ErrorTypes.AuthenticationFailed),
            status: StatusCodes.Status401Unauthorized,
            detail: exception.Message,
            instance: context.Request.Path,
            traceId: context.TraceIdentifier,
            extensions: extensions
        );
    }
}
