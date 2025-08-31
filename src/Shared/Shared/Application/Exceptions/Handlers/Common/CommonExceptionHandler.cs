using _116.Shared.Application.Exceptions.Entities;
using _116.Shared.Application.Exceptions.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace _116.Shared.Application.Exceptions.Handlers.Common;

/// <summary>
/// Handles a set of mapped framework/domain exceptions in a DRY, data-driven way.
/// </summary>
public sealed class CommonExceptionHandler(ILogger<CommonExceptionHandler> logger) : BaseErrorHandler(logger)
{
    private static readonly Dictionary<Type, (ErrorTypes Type, int Status)> Map = new()
    {
        { typeof(BadRequestException), (ErrorTypes.BadRequest, StatusCodes.Status400BadRequest) },
        { typeof(BadHttpRequestException), (ErrorTypes.BadRequest, StatusCodes.Status400BadRequest) },
        { typeof(NotFoundException), (ErrorTypes.NotFound, StatusCodes.Status404NotFound) },
        { typeof(InvalidOperationException), (ErrorTypes.BadRequest, StatusCodes.Status400BadRequest) },
        { typeof(ArgumentException), (ErrorTypes.BadRequest, StatusCodes.Status400BadRequest) },
        { typeof(InternalServerException), (ErrorTypes.InternalError, StatusCodes.Status500InternalServerError) }
    };

    /// <inheritdoc />
    public override int Priority => 10;

    /// <inheritdoc />
    public override bool CanHandle(Exception exception) => Map.ContainsKey(exception.GetType());

    /// <inheritdoc />
    protected override ErrorResponseEntity CreateErrorResponse(Exception exception, HttpContext context)
    {
        (ErrorTypes Type, int Status) mapping = Map[exception.GetType()];
        return ErrorResponse(mapping.Type, mapping.Status, exception.Message, context);
    }
}
