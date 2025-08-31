using _116.Shared.Application.ErrorHandling.Models;
using Microsoft.AspNetCore.Http;

namespace _116.Shared.Application.ErrorHandling.Mappers.Contracts;

/// <summary>
/// Defines the contract for mapping specific error types to standardized error responses.
/// </summary>
/// <remarks>
/// This interface implements the Strategy pattern, allowing different error mappers
/// to handle specific error types (exceptions, authentication, validation, etc.)
/// while maintaining a consistent error response format across the application.
/// </remarks>
public interface IErrorMapper
{
    /// <summary>
    /// Determines whether this mapper can handle the specified error object.
    /// </summary>
    /// <param name="error">The error object to evaluate</param>
    /// <returns>True if this mapper can handle the error; otherwise, false</returns>
    bool CanHandle(object error);

    /// <summary>
    /// Determines whether this mapper can handle the specified error type.
    /// </summary>
    /// <param name="errorType">The error type to evaluate</param>
    /// <returns>True if this mapper can handle the error type; otherwise, false</returns>
    bool CanHandle(Type errorType);

    /// <summary>
    /// Maps the error to a standardized error response.
    /// </summary>
    /// <param name="error">The error to map</param>
    /// <param name="context">The current HTTP context</param>
    /// <returns>A standardized error response</returns>
    ErrorResponse MapToErrorResponse(object error, HttpContext context);

    /// <summary>
    /// Gets the priority of this mapper when multiple mappers can handle the same error.
    /// </summary>
    /// <remarks>
    /// Higher values indicate higher priority. When multiple mappers can handle the same error,
    /// the one with the highest priority will be used. Default implementations should return 0.
    /// Specialized mappers should return higher values to take precedence.
    /// </remarks>
    int Priority => 0;
}

/// <summary>
/// Generic version of IErrorMapper for type-safe error mapping.
/// </summary>
/// <typeparam name="TError">The specific error type this mapper handles</typeparam>
public interface IErrorMapper<in TError> : IErrorMapper where TError : class
{
    /// <summary>
    /// Maps the strongly typed error to a standardized error response.
    /// </summary>
    /// <param name="error">The strongly typed error to map</param>
    /// <param name="context">The current HTTP context</param>
    /// <returns>A standardized error response</returns>
    ErrorResponse MapToErrorResponse(TError error, HttpContext context);

    /// <summary>
    /// Implementation of non-generic interface method.
    /// </summary>
    ErrorResponse IErrorMapper.MapToErrorResponse(object error, HttpContext context)
    {
        if (error is TError typedError) return MapToErrorResponse(typedError, context);

        throw new ArgumentException($"Expected error of type {typeof(TError).Name} but got {error.GetType().Name}");
    }

    /// <summary>
    /// Implementation of CanHandle for the generic interface.
    /// </summary>
    bool IErrorMapper.CanHandle(object error) => error is TError;

    /// <summary>
    /// Implementation of CanHandle for the generic interface.
    /// </summary>
    bool IErrorMapper.CanHandle(Type errorType) => typeof(TError).IsAssignableFrom(errorType);
}
