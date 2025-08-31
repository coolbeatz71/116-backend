using System.Text.Json;
using _116.Shared.Application.ErrorHandling.Enums;
using _116.Shared.Application.ErrorHandling.Mappers.Contracts;
using _116.Shared.Application.ErrorHandling.Models;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace _116.Shared.Application.ErrorHandling;

/// <summary>
/// Simple error handler that processes exceptions using registered error mappers.
/// </summary>
public sealed class ErrorPipelineHandler : IExceptionHandler
{
    private readonly IEnumerable<IErrorMapper> _errorMappers;
    private readonly ILogger<ErrorPipelineHandler> _logger;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    public ErrorPipelineHandler(IEnumerable<IErrorMapper> errorMappers, ILogger<ErrorPipelineHandler> logger)
    {
        _errorMappers = errorMappers.OrderByDescending(m => m.Priority);
        _logger = logger;
    }

    /// <summary>
    /// Handles exceptions through the ASP.NET Core exception handling pipeline.
    /// </summary>
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            IErrorMapper mapper = FindMapper(exception);
            ErrorResponse errorResponse = mapper.MapToErrorResponse(exception, httpContext);

            await WriteJsonResponseAsync(httpContext, errorResponse, cancellationToken);
            return true;
        }
        catch (Exception handlerException)
        {
            _logger.LogCritical(
                handlerException,
                "Critical error while processing exception {OriginalException} for {RequestPath}",
                exception.GetType().Name,
                httpContext.Request.Path
            );

            await WriteFallbackResponseAsync(httpContext, cancellationToken);
            return true;
        }
    }

    /// <summary>
    /// Finds the appropriate error mapper for the given exception.
    /// </summary>
    private IErrorMapper FindMapper(Exception exception)
    {
        IErrorMapper? mapper = _errorMappers.FirstOrDefault(m => m.CanHandle(exception));
        return mapper ?? new FallbackErrorMapper(_logger);
    }

    /// <summary>
    /// Writes error response as JSON to HTTP context.
    /// </summary>
    private static async Task WriteJsonResponseAsync(
        HttpContext context,
        ErrorResponse errorResponse,
        CancellationToken cancellationToken
    )
    {
        context.Response.StatusCode = errorResponse.Status;
        context.Response.ContentType = "application/json";

        string json = JsonSerializer.Serialize(errorResponse, JsonOptions);
        await context.Response.WriteAsync(json, cancellationToken);
    }

    /// <summary>
    /// Writes minimal fallback response when the handler itself fails.
    /// </summary>
    private static async Task WriteFallbackResponseAsync(
        HttpContext context,
        CancellationToken cancellationToken
    )
    {
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        context.Response.ContentType = "application/json";

        var fallbackResponse = new
        {
            title = ErrorTypes.InternalError,
            status = StatusCodes.Status500InternalServerError,
            detail = "An unexpected error occurred while processing your request.",
            instance = context.Request.Path.ToString(),
            traceId = context.TraceIdentifier,
            timestamp = DateTimeOffset.UtcNow
        };

        string json = JsonSerializer.Serialize(fallbackResponse, JsonOptions);
        await context.Response.WriteAsync(json, cancellationToken);
    }

    /// <summary>
    /// Fallback error mapper for unhandled exception types.
    /// </summary>
    private sealed class FallbackErrorMapper(ILogger logger) : IErrorMapper
    {
        public int Priority => -1;

        public bool CanHandle(object error) => true;
        public bool CanHandle(Type errorType) => true;

        public ErrorResponse MapToErrorResponse(object error, HttpContext context)
        {
            logger.LogWarning(
                "Using fallback mapper for unhandled error type {ErrorType} at {RequestPath}",
                error.GetType().Name, context.Request.Path
            );

            string message = error switch
            {
                Exception ex => ex.Message,
                _ => "An unexpected error occurred while processing your request."
            };

            return ErrorResponse.Create(
                title: nameof(ErrorTypes.InternalError),
                status: StatusCodes.Status500InternalServerError,
                detail: message,
                instance: context.Request.Path,
                traceId: context.TraceIdentifier
            );
        }
    }
}
