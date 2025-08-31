using _116.Shared.Application.ErrorHandling.Enums;
using _116.Shared.Application.ErrorHandling.Mappers.Contracts;
using _116.Shared.Application.ErrorHandling.Models;
using _116.Shared.Application.Exceptions;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace _116.Shared.Application.ErrorHandling.Mappers;

/// <summary>
/// Maps standard exceptions to structured error responses using a clean, data-driven approach.
/// </summary>
public sealed class ExceptionErrorMapper(ILogger<ExceptionErrorMapper> logger)
    : BaseErrorMapper(logger), IErrorMapper<Exception>
{
    private static readonly Dictionary<Type, (ErrorTypes ErrorType, int StatusCode)> ExceptionMappings = new()
    {
        { typeof(BadRequestException), (ErrorTypes.BadRequest, StatusCodes.Status400BadRequest) },
        { typeof(BadHttpRequestException), (ErrorTypes.BadRequest, StatusCodes.Status400BadRequest) },
        { typeof(NotFoundException), (ErrorTypes.NotFound, StatusCodes.Status404NotFound) },
        { typeof(UnauthorizedAccessException), (ErrorTypes.AuthenticationFailed, StatusCodes.Status401Unauthorized) },
        { typeof(InvalidOperationException), (ErrorTypes.BadRequest, StatusCodes.Status400BadRequest) },
        { typeof(ArgumentException), (ErrorTypes.BadRequest, StatusCodes.Status400BadRequest) },
        { typeof(InternalServerException), (ErrorTypes.InternalError, StatusCodes.Status500InternalServerError) }
    };

    public override int Priority => 0;

    public override bool CanHandle(object error) => error is Exception;

    public override bool CanHandle(Type errorType) => typeof(Exception).IsAssignableFrom(errorType);

    public ErrorResponse MapToErrorResponse(Exception exception, HttpContext context)
    {
        return MapToErrorResponse((object)exception, context);
    }

    protected override ErrorResponse CreateErrorResponse(object error, HttpContext context)
    {
        return error switch
        {
            ValidationException validationEx => CreateValidationErrorResponse(validationEx, context),
            Exception ex => CreateStandardErrorResponse(ex, context),
            _ => CreateSimpleErrorResponse(
                ErrorTypes.InternalError,
                StatusCodes.Status500InternalServerError,
                "An unexpected error occurred.",
                context
            )
        };
    }

    private static ErrorResponse CreateValidationErrorResponse(ValidationException exception, HttpContext context)
    {
        Dictionary<string, string[]> validationErrors = exception.Errors
            .GroupBy(e => e.PropertyName)
            .ToDictionary(
                g => g.Key,
                g => g.Select(e => e.ErrorMessage).ToArray()
            );

        var extensions = new Dictionary<string, object> { ["errors"] = validationErrors };

        return CreateExtendedErrorResponse(
            ErrorTypes.ValidationFailed,
            StatusCodes.Status400BadRequest,
            exception.Message,
            context,
            extensions
        );
    }

    private static ErrorResponse CreateStandardErrorResponse(Exception exception, HttpContext context)
    {
        Type exceptionType = exception.GetType();

        if (ExceptionMappings.TryGetValue(exceptionType, out (ErrorTypes ErrorType, int StatusCode) mapping))
        {
            return CreateSimpleErrorResponse(mapping.ErrorType, mapping.StatusCode, exception.Message, context);
        }

        // Fallback for unmapped exceptions
        return CreateSimpleErrorResponse(
            ErrorTypes.InternalError,
            StatusCodes.Status500InternalServerError,
            exception.Message,
            context
        );
    }
}
