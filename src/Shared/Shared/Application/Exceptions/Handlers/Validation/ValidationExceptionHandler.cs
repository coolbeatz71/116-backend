using _116.Shared.Application.Exceptions.Entities;
using _116.Shared.Application.Exceptions.Enums;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace _116.Shared.Application.Exceptions.Handlers.Validation;

/// <summary>
/// Handles <see cref="ValidationException"/> into a 400 response with grouped errors.
/// </summary>
public sealed class ValidationExceptionHandler(ILogger<ValidationExceptionHandler> logger) : BaseErrorHandler(logger)
{
    /// <inheritdoc />
    public override int Priority => 90;

    /// <inheritdoc />
    public override bool CanHandle(Exception exception) => exception is ValidationException;

    /// <inheritdoc />
    protected override ErrorResponseEntity CreateErrorResponse(Exception exception, HttpContext context)
    {
        var ex = (ValidationException)exception;
        Dictionary<string, string[]> validationErrors = ex.Errors
            .GroupBy(e => e.PropertyName)
            .ToDictionary(
                g => g.Key,
                g => g.Select(e => e.ErrorMessage).ToArray()
            );

        var extensions = new Dictionary<string, object> { ["errors"] = validationErrors };
        return Extended(
            ErrorTypes.ValidationFailed,
            StatusCodes.Status400BadRequest,
            ex.Message, context, extensions
        );
    }
}
