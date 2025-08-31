using _116.Shared.Application.Exceptions.Entities;
using Microsoft.AspNetCore.Http;

namespace _116.Shared.Application.Exceptions.Abstractions;

/// <summary>
/// Defines a single responsibility error handler used in a Chain of Responsibility.
/// </summary>
public interface IErrorHandler
{
    /// <summary>
    /// Higher values have precedence when the chain is constructed.
    /// </summary>
    int Priority { get; }

    /// <summary>
    /// Determines whether this handler can process the provided <paramref name="exception"/>.
    /// </summary>
    bool CanHandle(Exception exception);

    /// <summary>
    /// Attempts to handle the exception.
    /// </summary>
    /// <param name="exception">The exception to handle.</param>
    /// <param name="context">The HTTP context for request/response details.</param>
    /// <param name="errorResponse">The produced <see cref="ErrorResponseEntity"/> if handling succeeds.</param>
    /// <returns><c>true</c> if handled; otherwise <c>false</c>.</returns>
    bool TryHandle(Exception exception, HttpContext context, out ErrorResponseEntity errorResponse);
}
