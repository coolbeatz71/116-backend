using _116.Core.Application.ErrorHandling.Models;
using Microsoft.AspNetCore.Http;

namespace _116.Core.Application.ErrorHandling.Abstractions;

/// <summary>
/// Defines the contract for unified error handling across the application.
/// </summary>
/// <remarks>
/// This interface provides a consistent way to handle all types of errors including exceptions,
/// authentication failures, validation errors, and custom business logic errors.
/// Implementations should ensure proper logging, error response formatting, and status code setting.
/// </remarks>
public interface IErrorHandler
{
    /// <summary>
    /// Handles an exception and generates an appropriate error response.
    /// </summary>
    /// <param name="context">The current HTTP context</param>
    /// <param name="exception">The exception to handle</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>True if the error was handled successfully; otherwise, false</returns>
    ValueTask<bool> TryHandleAsync(
        HttpContext context,
        Exception exception,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Handles an error scenario and writes the response directly to the HTTP context.
    /// </summary>
    /// <param name="context">The current HTTP context</param>
    /// <param name="errorType">The type of error being handled</param>
    /// <param name="message">The error message</param>
    /// <param name="statusCode">The HTTP status code</param>
    /// <param name="extensions">Additional error-specific data</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    Task HandleErrorAsync(
        HttpContext context,
        string errorType,
        string message,
        int statusCode,
        Dictionary<string, object>? extensions = null,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Creates an error response without writing it to the HTTP context.
    /// </summary>
    /// <param name="context">The current HTTP context</param>
    /// <param name="errorType">The type of error</param>
    /// <param name="message">The error message</param>
    /// <param name="statusCode">The HTTP status code</param>
    /// <param name="extensions">Additional error-specific data</param>
    /// <returns>A structured error response</returns>
    ErrorResponse CreateErrorResponse(
        HttpContext context,
        string errorType,
        string message,
        int statusCode,
        Dictionary<string, object>? extensions = null
    );
}
