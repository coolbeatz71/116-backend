using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace _116.Core.Application.Exceptions.Handlers;

/// <summary>
/// Handles unhandled exceptions thrown in the application and returns structured <see cref="ProblemDetails"/> responses.
/// </summary>
/// <remarks>
/// This handler intercepts exceptions globally and maps known exception types (e.g., <see cref="ValidationException"/>,
/// <see cref="NotFoundException"/>)
/// to standardized error responses. Additional metadata such as trace ID
/// and validation errors are included when applicable.
/// To enable this handler, register it in the service container and middleware pipeline:
/// <code>
/// <![CDATA[
/// builder.Services.AddExceptionHandler<CustomExceptionHandler>();
/// app.UseExceptionHandler();
/// ]]>
/// </code>
/// </remarks>
public class CustomExceptionHandler(ILogger<CustomExceptionHandler> logger) : IExceptionHandler
{
    /// <summary>
    /// Attempts to handle an unhandled exception and convert it to a <see cref="ProblemDetails"/> HTTP response.
    /// </summary>
    /// <param name="context">The current <see cref="HttpContext"/>.</param>
    /// <param name="exception">The unhandled <see cref="Exception"/>.</param>
    /// <param name="cancellationToken">Cancellation token for the request.</param>
    /// <returns><c>true</c> if the exception was handled; otherwise, <c>false</c>.</returns>
    public async ValueTask<bool> TryHandleAsync(
        HttpContext context,
        Exception exception,
        CancellationToken cancellationToken)
    {
        logger.LogError(
            "Error Message: {Message}, Time of Occurrence: {Time}",
            exception.Message,
            DateTime.UtcNow
        );

        ProblemDetails problemDetails = GetProblemDetailsFromException(exception, context);

        // Enrich problem response
        problemDetails.Instance = context.Request.Path;
        problemDetails.Extensions["traceId"] = context.TraceIdentifier;

        // Include validation errors if applicable
        if (exception is ValidationException validationException)
        {
            problemDetails.Extensions["errors"] = validationException.Errors;
        }

        context.Response.StatusCode = problemDetails.Status ?? StatusCodes.Status500InternalServerError;
        await context.Response.WriteAsJsonAsync(problemDetails, cancellationToken: cancellationToken);
        return true;
    }

    /// <summary>
    /// Maps known exception types to their corresponding <see cref="ProblemDetails"/> representation.
    /// </summary>
    /// <param name="exception">The exception to map.</param>
    /// <param name="context">The current HTTP context.</param>
    /// <returns>A <see cref="ProblemDetails"/> instance describing the error.</returns>
    /// <example>
    /// For a <see cref="NotFoundException"/>:
    /// <code>
    /// new ProblemDetails
    /// {
    ///     Title = "NotFoundException",
    ///     Detail = "The requested entity was not found.",
    ///     Status = 404
    /// }
    /// </code>
    /// </example>
    private static ProblemDetails GetProblemDetailsFromException(
        Exception exception,
        HttpContext context
    ) =>
        exception switch
        {
            ValidationException ex => CreateProblemDetails(
                ex.Message,
                nameof(ValidationException),
                StatusCodes.Status400BadRequest
            ),

            BadRequestException ex => CreateProblemDetails(
                ex.Message,
                nameof(BadRequestException),
                StatusCodes.Status400BadRequest
            ),

            BadHttpRequestException ex => CreateProblemDetails(
                ex.Message,
                nameof(BadHttpRequestException),
                StatusCodes.Status400BadRequest
            ),

            NotFoundException ex => CreateProblemDetails(
                ex.Message, nameof(NotFoundException),
                StatusCodes.Status404NotFound
            ),

            InternalServerException ex => CreateProblemDetails(
                ex.Message,
                nameof(InternalServerException),
                StatusCodes.Status500InternalServerError
            ),

            _ => CreateProblemDetails(
                exception.Message,
                exception.GetType().Name,
                StatusCodes.Status500InternalServerError
            )
        };

    /// <summary>
    /// Creates a new <see cref="ProblemDetails"/> object using provided values.
    /// </summary>
    /// <param name="detail">A human-readable explanation of the error.</param>
    /// <param name="title">A short title for the error type.</param>
    /// <param name="statusCode">The HTTP status code.</param>
    /// <returns>A populated <see cref="ProblemDetails"/> object.</returns>
    private static ProblemDetails CreateProblemDetails(string detail, string title, int statusCode)
    {
        return new ProblemDetails
        {
            Title = title,
            Detail = detail,
            Status = statusCode
        };
    }
}
