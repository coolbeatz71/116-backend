using System.Security.Claims;
using _116.Shared.Application.Exceptions.Entities;
using _116.Shared.Application.Exceptions.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace _116.Shared.Application.Exceptions.Handlers.Authentication;

/// <summary>
/// Handles authorization failures and returns 403 with required permission context.
/// </summary>
public sealed class AuthorizationExceptionHandler(ILogger<AuthorizationExceptionHandler> logger) : BaseErrorHandler(logger)
{
    /// <inheritdoc />
    public override int Priority => 105;

    /// <inheritdoc />
    public override bool CanHandle(Exception exception) => exception is AuthorizationException;

    /// <inheritdoc />
    protected override ErrorResponseEntity CreateErrorResponse(Exception exception, HttpContext context)
    {
        var ex = (AuthorizationException)exception;

        string[] userRoles = context.User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToArray();
        var extensions = new Dictionary<string, object>
        {
            ["requiredPermission"] = ex.RequiredPermission,
            ["userRoles"] = userRoles
        };

        return Extended(ErrorTypes.AccessDenied, StatusCodes.Status403Forbidden, ex.Message, context, extensions);
    }
}
