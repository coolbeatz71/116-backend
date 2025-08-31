using _116.Shared.Application.Exceptions.Abstractions;
using _116.Shared.Application.Exceptions.Entities;
using _116.Shared.Application.Exceptions.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace _116.Shared.Application.Exceptions.Handlers;

/// <summary>
/// Base class with helpers for implementing <see cref="IErrorHandler"/> without duplication.
/// </summary>
public abstract class BaseErrorHandler(ILogger logger) : IErrorHandler
{
    /// <summary>Shared logger instance.</summary>
    protected readonly ILogger Logger = logger;

    /// <inheritdoc />
    public abstract int Priority { get; }

    /// <inheritdoc />
    public abstract bool CanHandle(Exception exception);

    /// <inheritdoc />
    public bool TryHandle(
        Exception exception,
        HttpContext context,
        out ErrorResponseEntity errorResponse
    )
    {
        if (!CanHandle(exception))
        {
            errorResponse = null!;
            return false;
        }

        Log(exception, context);
        errorResponse = CreateErrorResponse(exception, context);
        return true;
    }

    /// <summary>
    /// Creates the error response for the given exception.
    /// </summary>
    protected abstract ErrorResponseEntity CreateErrorResponse(Exception exception, HttpContext context);

    /// <summary>
    /// Helper to create a simple error response.
    /// </summary>
    protected static ErrorResponseEntity ErrorResponse(
        ErrorTypes type,
        int statusCode,
        string message,
        HttpContext context
    )
    {
        return ErrorResponseEntity.Create(
            type.ToString(),
            statusCode,
            message,
            context.Request.Path,
            context.TraceIdentifier
        );
    }

    /// <summary>
    /// Helper to create an extended error response.
    /// </summary>
    protected static ErrorResponseEntity Extended(
        ErrorTypes type,
        int statusCode,
        string message,
        HttpContext context,
        Dictionary<string, object> extensions
    )
    {
        return ErrorResponseEntity.CreateWithExtensions(
            type.ToString(),
            statusCode,
            message,
            context.Request.Path,
            context.TraceIdentifier,
            extensions
        );
    }

    /// <summary>
    /// Centralized logging logic with consistent templates and levels.
    /// </summary>
    private void Log(Exception exception, HttpContext context)
    {
        LogLevel level = exception switch
        {
            ArgumentException => LogLevel.Warning,
            InvalidOperationException => LogLevel.Warning,
            UnauthorizedAccessException => LogLevel.Warning,
            _ when exception.ToString().Contains("validation", StringComparison.OrdinalIgnoreCase)
                => LogLevel.Warning,
            _ => LogLevel.Error
        };

        const string template = "{ErrorType} occurred at {RequestPath}. Exception: {ExceptionMessage}";

        Logger.Log(
            level,
            exception,
            template,
            exception.GetType().Name,
            context.Request.Path,
            exception.Message
        );
    }
}
