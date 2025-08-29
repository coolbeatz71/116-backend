using _116.Core.Application.ErrorHandling.Abstractions;
using _116.Core.Application.ErrorHandling.Models;
using _116.Core.Application.Exceptions;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace _116.Core.Application.ErrorHandling.Mappers;

/// <summary>
/// Maps standard exceptions to structured error responses.
/// </summary>
/// <remarks>
/// This mapper handles common application exceptions including validation errors,
/// not found exceptions, bad request exceptions, and general system exceptions.
/// It provides consistent error response formatting and appropriate HTTP status codes.
/// </remarks>
public sealed class ExceptionErrorMapper(ILogger<ExceptionErrorMapper> logger) : IErrorMapper<Exception>
{
    /// <summary>
    /// Gets the priority of this mapper (0 = default priority).
    /// </summary>
    public int Priority => 0;

    /// <summary>
    /// Maps an exception to a standardized error response.
    /// </summary>
    /// <param name="exception">The exception to map</param>
    /// <param name="context">The current HTTP context</param>
    /// <returns>A structured error response</returns>
    public ErrorResponse MapToErrorResponse(
        Exception exception,
        HttpContext context
    )
    {
        logger.LogError(exception,
            "Exception occurred in {RequestPath}. Exception: {ExceptionType}",
            context.Request.Path,
            exception.GetType().Name
        );

        return exception switch
        {
            ValidationException validationEx => CreateValidationErrorResponse(validationEx, context),
            BadRequestException badRequestEx => CreateBadRequestErrorResponse(badRequestEx, context),
            NotFoundException notFoundEx => CreateNotFoundErrorResponse(notFoundEx, context),
            UnauthorizedAccessException unauthorizedEx => CreateUnauthorizedErrorResponse(unauthorizedEx, context),
            InvalidOperationException invalidOpEx => CreateInvalidOperationErrorResponse(invalidOpEx, context),
            ArgumentException argumentEx => CreateArgumentErrorResponse(argumentEx, context),
            InternalServerException internalServerEx => CreateInternalServerErrorResponse(internalServerEx, context),
            _ => CreateGenericErrorResponse(exception, context)
        };
    }

    /// <summary>
    /// Creates an error response for validation exceptions.
    /// </summary>
    private static ErrorResponse CreateValidationErrorResponse(
        ValidationException exception,
        HttpContext context
    )
    {
        var extensions = new Dictionary<string, object>
        {
            ["errors"] = exception.Errors.GroupBy(e => e.PropertyName)
                .ToDictionary(
                    group => group.Key,
                    group => group.Select(e => e.ErrorMessage).ToArray()
                )
        };

        return ErrorResponse.CreateWithExtensions(
            title: "Validation Error",
            status: StatusCodes.Status400BadRequest,
            detail: "One or more validation errors occurred.",
            instance: context.Request.Path,
            traceId: context.TraceIdentifier,
            extensions: extensions
        );
    }

    /// <summary>
    /// Creates an error response for bad request exceptions.
    /// </summary>
    private static ErrorResponse CreateBadRequestErrorResponse(
        BadRequestException exception,
        HttpContext context
    )
    {
        return ErrorResponse.Create(
            title: "Bad Request",
            status: StatusCodes.Status400BadRequest,
            detail: exception.Message,
            instance: context.Request.Path,
            traceId: context.TraceIdentifier
        );
    }

    /// <summary>
    /// Creates an error response for not found exceptions.
    /// </summary>
    private static ErrorResponse CreateNotFoundErrorResponse(
        NotFoundException exception,
        HttpContext context
    )
    {
        return ErrorResponse.Create(
            title: "Resource Not Found",
            status: StatusCodes.Status404NotFound,
            detail: exception.Message,
            instance: context.Request.Path,
            traceId: context.TraceIdentifier
        );
    }

    /// <summary>
    /// Creates an error response for unauthorized access exceptions.
    /// </summary>
    private static ErrorResponse CreateUnauthorizedErrorResponse(
        UnauthorizedAccessException exception,
        HttpContext context
    )
    {
        return ErrorResponse.Create(
            title: "Unauthorized Access",
            status: StatusCodes.Status401Unauthorized,
            detail: exception.Message,
            instance: context.Request.Path,
            traceId: context.TraceIdentifier
        );
    }

    /// <summary>
    /// Creates an error response for invalid operation exceptions.
    /// </summary>
    private static ErrorResponse CreateInvalidOperationErrorResponse(
        InvalidOperationException exception,
        HttpContext context
    )
    {
        return ErrorResponse.Create(
            title: "Invalid Operation",
            status: StatusCodes.Status400BadRequest,
            detail: exception.Message,
            instance: context.Request.Path,
            traceId: context.TraceIdentifier
        );
    }

    /// <summary>
    /// Creates an error response for argument exceptions.
    /// </summary>
    private static ErrorResponse CreateArgumentErrorResponse(
        ArgumentException exception,
        HttpContext context
    )
    {
        return ErrorResponse.Create(
            title: "Invalid Argument",
            status: StatusCodes.Status400BadRequest,
            detail: exception.Message,
            instance: context.Request.Path,
            traceId: context.TraceIdentifier
        );
    }

    /// <summary>
    /// Creates an error response for internal server exceptions.
    /// </summary>
    private static ErrorResponse CreateInternalServerErrorResponse(
        InternalServerException exception,
        HttpContext context
    )
    {
        return ErrorResponse.Create(
            title: "Internal Server Error",
            status: StatusCodes.Status500InternalServerError,
            detail: exception.Message,
            instance: context.Request.Path,
            traceId: context.TraceIdentifier
        );
    }

    /// <summary>
    /// Creates a generic error response for unhandled exceptions.
    /// </summary>
    private static ErrorResponse CreateGenericErrorResponse(
        Exception exception,
        HttpContext context
    )
    {
        return ErrorResponse.Create(
            title: "Internal Server Error",
            status: StatusCodes.Status500InternalServerError,
            detail: exception.Message,
            instance: context.Request.Path,
            traceId: context.TraceIdentifier
        );
    }
}
