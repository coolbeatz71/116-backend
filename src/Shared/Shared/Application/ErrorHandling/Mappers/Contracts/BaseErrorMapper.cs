using _116.Shared.Application.ErrorHandling.Enums;
using _116.Shared.Application.ErrorHandling.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace _116.Shared.Application.ErrorHandling.Mappers.Contracts;

/// <summary>
/// Base error mapper that provides common functionality and eliminates code duplication.
/// </summary>
public abstract class BaseErrorMapper(ILogger logger) : IErrorMapper
{
    protected readonly ILogger Logger = logger;

    public abstract int Priority { get; }
    public abstract bool CanHandle(object error);
    public abstract bool CanHandle(Type errorType);

    public ErrorResponse MapToErrorResponse(object error, HttpContext context)
    {
        LogError(error, context);
        return CreateErrorResponse(error, context);
    }

    /// <summary>
    /// Creates the appropriate error response for the given error.
    /// </summary>
    protected abstract ErrorResponse CreateErrorResponse(object error, HttpContext context);

    /// <summary>
    /// Logs the error with the appropriate level and context.
    /// </summary>
    private void LogError(object error, HttpContext context)
    {
        LogLevel logLevel = GetLogLevel(error);
        string message = GetLogMessage(error);

        Logger.Log(logLevel, error as Exception, message, context.Request.Path, error.GetType().Name);
    }

    /// <summary>
    /// Gets the appropriate log level for the error type.
    /// </summary>
    private static LogLevel GetLogLevel(object error)
    {
        return error switch
        {
            ArgumentException => LogLevel.Warning,
            InvalidOperationException => LogLevel.Warning,
            UnauthorizedAccessException => LogLevel.Warning,
            _ when error.ToString()?.Contains(
                "validation",
                StringComparison.OrdinalIgnoreCase
            ) == true => LogLevel.Warning,
            _ => LogLevel.Error
        };
    }

    /// <summary>
    /// Gets the log message template for the error.
    /// </summary>
    private static string GetLogMessage(object error)
    {
        return error switch
        {
            UnauthorizedAccessException => "Unauthorized access attempt for {RequestPath}. Error: {ErrorType}",
            _ when error.ToString()?.Contains(
                "validation",
                StringComparison.OrdinalIgnoreCase
            ) == true => "Validation failed for {RequestPath}. Error: {ErrorType}",
            _ => "Exception occurred in {RequestPath}. Exception: {ErrorType}"
        };
    }

    /// <summary>
    /// Helper method to create simple error responses.
    /// </summary>
    protected static ErrorResponse CreateSimpleErrorResponse(
        ErrorTypes errorType,
        int statusCode,
        string message,
        HttpContext context
    )
    {
        return ErrorResponse.Create(
            title: errorType.ToString(),
            status: statusCode,
            detail: message,
            instance: context.Request.Path,
            traceId: context.TraceIdentifier
        );
    }

    /// <summary>
    /// Helper method to create error responses with extensions.
    /// </summary>
    protected static ErrorResponse CreateExtendedErrorResponse(
        ErrorTypes errorType,
        int statusCode,
        string message,
        HttpContext context,
        Dictionary<string, object> extensions
    )
    {
        return ErrorResponse.CreateWithExtensions(
            title: errorType.ToString(),
            status: statusCode,
            detail: message,
            instance: context.Request.Path,
            traceId: context.TraceIdentifier,
            extensions: extensions
        );
    }
}
