using _116.Shared.Application.ErrorHandling.Abstractions;
using _116.Shared.Application.ErrorHandling.Models;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace _116.Shared.Application.ErrorHandling;

/// <summary>
/// Unified error handler that processes all types of application errors through a consistent pipeline.
/// </summary>
/// <remarks>
/// This handler implements both IErrorHandler and IExceptionHandler interfaces to provide
/// unified error processing for exceptions, authentication failures, validation errors,
/// and custom business logic errors. It uses the Strategy pattern with error mappers
/// to handle different error types while maintaining consistent response formatting.
///
/// The handler automatically selects the appropriate error mapper based on error type
/// and mapper priority, ensuring specialized handling for different scenarios while
/// providing fallback handling for unexpected errors.
/// </remarks>
public sealed class ErrorPipelineHandler(
    IEnumerable<IErrorMapper> errorMappers,
    ILogger<ErrorPipelineHandler> logger
) : IErrorHandler, IExceptionHandler
{
    private readonly IErrorMapper[] _errorMappers = errorMappers
        .OrderByDescending(m => m.Priority)
        .ToArray();

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    /// <summary>
    /// Handles exceptions through the ASP.NET Core exception handling the middleware pipeline.
    /// </summary>
    /// <param name="httpContext">The current HTTP context</param>
    /// <param name="exception">The unhandled exception</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>True if the exception was handled, otherwise, false</returns>
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            IErrorMapper mapper = FindErrorMapper(exception);
            ErrorResponse errorResponse = mapper.MapToErrorResponse(exception, httpContext);

            await WriteErrorResponseAsync(httpContext, errorResponse, cancellationToken);
            return true;
        }
        catch (Exception handlerException)
        {
            logger.LogCritical(
                handlerException,
                "Critical error in while processing exception {OriginalException} for {RequestPath}",
                exception.GetType().Name,
                httpContext.Request.Path
            );

            // Fallback to a basic error response if our handler fails
            await WriteFallbackErrorAsync(httpContext, cancellationToken);
            return true;
        }
    }

    /// <summary>
    /// Handles error scenarios and writes the response directly to the HTTP context.
    /// </summary>
    /// <param name="context">The current HTTP context</param>
    /// <param name="errorType">The type of error being handled</param>
    /// <param name="message">The error message</param>
    /// <param name="statusCode">The HTTP status code</param>
    /// <param name="extensions">Additional error-specific data</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    public async Task HandleErrorAsync(
        HttpContext context,
        string errorType,
        string message,
        int statusCode,
        Dictionary<string, object>? extensions = null,
        CancellationToken cancellationToken = default
    )
    {
        logger.LogWarning(
            "Handling error {ErrorType} for {RequestPath}: {Message}",
            errorType,
            context.Request.Path,
            message
        );

        ErrorResponse errorResponse = CreateErrorResponse(context, errorType, message, statusCode, extensions);
        await WriteErrorResponseAsync(context, errorResponse, cancellationToken);
    }

    /// <summary>
    /// Builds a standardized <see cref="ErrorResponse"/> without direct writing to the HTTP response stream.
    /// </summary>
    /// <param name="context">The current <see cref="HttpContext"/> associated with the request.</param>
    /// <param name="errorType">A string identifier describing the type of error (e.g., validation, authentication, server error).</param>
    /// <param name="message">A human-readable description of the error.</param>
    /// <param name="statusCode">The HTTP status code to associate with the error.</param>
    /// <param name="extensions">Optional additional metadata to include in the error response.</param>
    /// <returns>
    /// A structured <see cref="ErrorResponse"/> that encapsulates the provided error details.
    /// </returns>
    public ErrorResponse CreateErrorResponse(
        HttpContext context,
        string errorType,
        string message,
        int statusCode,
        Dictionary<string, object>? extensions = null
    )
    {
        return extensions is { Count: > 0 }
            ? ErrorResponse.CreateWithExtensions(
                errorType,
                statusCode,
                message,
                context.Request.Path,
                context.TraceIdentifier,
                extensions
            )
            : ErrorResponse.Create(
                errorType,
                statusCode,
                message,
                context.Request.Path,
                context.TraceIdentifier
            );
    }

    /// <summary>
    /// Finds the appropriate error mapper for the given error.
    /// </summary>
    /// <param name="error">The error to find a mapper for</param>
    /// <returns>The error mapper to use</returns>
    /// <remarks>
    /// Mappers are evaluated in priority order (highest first). The first mapper
    /// that can handle the error type is selected. If no specific mapper is found,
    /// a fallback mapper is used to ensure all errors are handled.
    /// </remarks>
    private IErrorMapper FindErrorMapper(object error)
    {
        IErrorMapper? mapper = _errorMappers.FirstOrDefault(m => m.CanHandle(error));

        if (mapper != null)
        {
            logger.LogDebug(
                "Using error mapper {MapperType} for error type {ErrorType}",
                mapper.GetType().Name,
                error.GetType().Name
            );
            return mapper;
        }

        logger.LogWarning(
            "No specific error mapper found for error type {ErrorType}, using fallback",
            error.GetType().Name
        );

        return new FallbackErrorMapper(logger);
    }

    /// <summary>
    /// Writes the error response to the HTTP context.
    /// </summary>
    /// <param name="context">The HTTP context</param>
    /// <param name="errorResponse">The error response to write</param>
    /// <param name="cancellationToken">Cancellation token</param>
    private static async Task WriteErrorResponseAsync(
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
    /// Writes a basic fallback error response when the handler itself fails.
    /// </summary>
    /// <param name="context">The HTTP context</param>
    /// <param name="cancellationToken">Cancellation token</param>
    private static async Task WriteFallbackErrorAsync(
        HttpContext context,
        CancellationToken cancellationToken
    )
    {
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        context.Response.ContentType = "application/json";

        var fallbackResponse = new
        {
            title = "Internal Server Error",
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
    /// Fallback error mapper used when no specific mapper can handle an error.
    /// </summary>
    private sealed class FallbackErrorMapper(ILogger logger) : IErrorMapper
    {
        public int Priority => -1; // Lowest priority

        public bool CanHandle(object error) => true; // Can handle any error

        public ErrorResponse MapToErrorResponse(object error, HttpContext context)
        {
            logger.LogError(
                "Using fallback error mapper for unhandled error type {ErrorType} at {RequestPath}",
                error.GetType().Name,
                context.Request.Path
            );

            string message = error switch
            {
                Exception ex => ex.Message,
                _ => "An unexpected error occurred while processing your request."
            };

            return ErrorResponse.Create(
                title: "Internal Server Error",
                status: StatusCodes.Status500InternalServerError,
                detail: message,
                instance: context.Request.Path,
                traceId: context.TraceIdentifier
            );
        }
    }
}
