using FluentValidation;
using FluentValidation.Results;
using MediatR;

namespace _116.Shared.Application.Behaviors;

/// <summary>
/// A MediatR pipeline behavior that validates incoming requests using FluentValidation.
/// Throws a <see cref="ValidationException"/> if any validation errors are found.
/// </summary>
/// <typeparam name="TRequest">The type of the request. Must implement <see cref="IRequest{TResponse}"/>.</typeparam>
/// <typeparam name="TResponse">The type of the response.</typeparam>
public class ValidationBehavior<TRequest, TResponse>
    (IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    /// <summary>
    /// Validates the incoming request using all registered <see cref="IValidator{TRequest}"/> implementations.
    /// If any validation errors are found, a <see cref="ValidationException"/> is thrown.
    /// </summary>
    /// <param name="request">The request to validate.</param>
    /// <param name="next">The next delegate in the MediatR pipeline.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The response from the next handler if validation passes.</returns>
    /// <exception cref="ValidationException">Thrown if any validation failures are found.</exception>
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken
    )
    {
        var context = new ValidationContext<TRequest>(request);

        ValidationResult[] validationResults = await Task.WhenAll(
            validators.Select(v => v.ValidateAsync(context, cancellationToken))
        );

        List<ValidationFailure> failures = validationResults
            .Where(r => r.Errors.Count != 0)
            .SelectMany(r => r.Errors)
            .ToList();

        if (failures.Count != 0) throw new ValidationException(failures);

        return await next(cancellationToken);
    }
}
