using _116.Shared.Application.Exceptions.Entities;
using _116.Shared.Application.Exceptions.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace _116.Shared.Application.Exceptions.Handlers.Authentication;

/// <summary>
/// Handles <see cref="JwtTokenException"/> and returns a detailed 4xx response.
/// </summary>
public sealed class JwtTokenExceptionHandler(ILogger<JwtTokenExceptionHandler> logger)
    : BaseErrorHandler(logger)
{
    /// <inheritdoc />
    public override int Priority => 120;

    /// <inheritdoc />
    public override bool CanHandle(Exception exception) => exception is JwtTokenException;

    /// <inheritdoc />
    protected override ErrorResponseEntity CreateErrorResponse(Exception exception, HttpContext context)
    {
        var ex = (JwtTokenException)exception;

        int statusCode = ex.Issue switch
        {
            JwtErrorTypes.Malformed => StatusCodes.Status400BadRequest,
            JwtErrorTypes.Expired => StatusCodes.Status403Forbidden,
            _ => StatusCodes.Status401Unauthorized
        };

        var extensions = new Dictionary<string, object>
        {
            ["tokenIssue"] = ex.Issue,
            ["isExpired"] = ex.IsExpired,
            ["tokenType"] = "Bearer"
        };

        return Extended(ErrorTypes.AuthenticationFailed, statusCode, ex.Message, context, extensions);
    }
}
