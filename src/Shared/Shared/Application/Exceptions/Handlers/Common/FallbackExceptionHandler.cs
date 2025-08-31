using _116.Shared.Application.Exceptions.Entities;
using _116.Shared.Application.Exceptions.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace _116.Shared.Application.Exceptions.Handlers.Common;

/// <summary>
/// Final chain handler: catches everything else and returns 500.
/// </summary>
public sealed class FallbackExceptionHandler(ILogger<FallbackExceptionHandler> logger) : BaseErrorHandler(logger)
{
    /// <inheritdoc />
    public override int Priority => -1;

    /// <inheritdoc />
    public override bool CanHandle(Exception exception) => true;

    /// <inheritdoc />
    protected override ErrorResponseEntity CreateErrorResponse(Exception exception, HttpContext context)
    {
        return ErrorResponse(
            ErrorTypes.InternalError,
            StatusCodes.Status500InternalServerError,
            exception.Message,
            context
        );
    }
}
