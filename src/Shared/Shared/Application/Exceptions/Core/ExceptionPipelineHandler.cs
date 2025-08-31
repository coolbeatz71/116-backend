using _116.Shared.Application.Exceptions.Abstractions;
using _116.Shared.Application.Exceptions.Entities;
using _116.Shared.Application.Exceptions.Enums;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace _116.Shared.Application.Exceptions.Core;

/// <summary>
/// Exception handler that delegates to a Chain of Responsibility of <see cref="IErrorHandler"/>.
/// </summary>
public sealed class ExceptionPipelineHandler : IExceptionHandler
{
    private readonly IReadOnlyList<IErrorHandler> _handlers;
    private readonly IErrorResponseWriter _writer;
    private readonly ILogger<ExceptionPipelineHandler> _logger;

    /// <summary>
    /// Initializes a new instance of <see cref="ExceptionPipelineHandler"/>.
    /// </summary>
    /// <param name="handlers">Registered handlers ordered by priority (desc).</param>
    /// <param name="writer">JSON writer for problem responses.</param>
    /// <param name="logger">Logger.</param>
    public ExceptionPipelineHandler(
        IEnumerable<IErrorHandler> handlers,
        IErrorResponseWriter writer,
        ILogger<ExceptionPipelineHandler> logger
    )
    {
        // Order handlers once (high → low) to form a stable chain without hard-coding the order.
        _handlers = handlers.OrderByDescending(h => h.Priority).ToArray();
        _writer = writer;
        _logger = logger;
    }

    /// <inheritdoc />
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            foreach (IErrorHandler handler in _handlers)
            {
                if (!handler.TryHandle(exception, httpContext, out ErrorResponseEntity errorResponse))
                {
                    continue;
                }

                await _writer.WriteAsync(httpContext, errorResponse, cancellationToken);
                return true;
            }

            // Should never happen because of FallbackErrorHandler, but just in case.
            var fallback = ErrorResponseEntity.Create(
                title: nameof(ErrorTypes.InternalError),
                status: StatusCodes.Status500InternalServerError,
                detail: "An unexpected error occurred while processing your request.",
                instance: httpContext.Request.Path,
                traceId: httpContext.TraceIdentifier
            );

            await _writer.WriteAsync(httpContext, fallback, cancellationToken);
            return true;
        }
        catch (Exception handlerException)
        {
            _logger.LogCritical(handlerException,
                "Critical error while processing exception {OriginalException} for {RequestPath}",
                exception.GetType().Name,
                httpContext.Request.Path
            );

            // Minimal fallback when even the handler fails.
            httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
            httpContext.Response.ContentType = "application/json";

            var minimal = new
            {
                title = ErrorTypes.InternalError,
                status = StatusCodes.Status500InternalServerError,
                detail = "An unexpected error occurred while processing your request.",
                instance = httpContext.Request.Path.ToString(),
                traceId = httpContext.TraceIdentifier,
                timestamp = DateTimeOffset.UtcNow
            };

            await httpContext.Response.WriteAsJsonAsync(minimal, cancellationToken);
            return true;
        }
    }
}
