using _116.Shared.Application.ErrorHandling.Abstractions;

namespace _116.Shared.Application.ErrorHandling.Configuration;

/// <summary>
/// Configuration options for the unified error handling system.
/// </summary>
/// <remarks>
/// This class allows for customization of the error handling system behavior,
/// including the registration of custom error mappers and configuration of
/// error processing options.
/// </remarks>
public sealed class ErrorHandlingOptions
{
    /// <summary>
    /// Gets the list of custom error mapper types to register.
    /// </summary>
    /// <remarks>
    /// Custom error mappers will be registered in addition to the default mappers.
    /// They should implement <see cref="IErrorMapper"/> and define appropriate
    /// priority values for proper order of evaluation.
    /// </remarks>
    public List<Type> CustomMappers { get; } = [];

    /// <summary>
    /// Adds a custom error mapper type to be registered with the error handling system.
    /// </summary>
    /// <typeparam name="TMapper">The type of error mapper to add</typeparam>
    /// <returns>The option's instance for method chaining</returns>
    /// <remarks>
    /// The specified mapper type must implement <see cref="IErrorMapper"/> and have
    /// a constructor compatible with dependency injection.
    /// </remarks>
    public ErrorHandlingOptions AddMapper<TMapper>() where TMapper : class, IErrorMapper
    {
        CustomMappers.Add(typeof(TMapper));
        return this;
    }

    /// <summary>
    /// Adds a custom error mapper type to be registered with the error handling system.
    /// </summary>
    /// <param name="mapperType">The type of error mapper to add</param>
    /// <returns>The option's instance for method chaining</returns>
    /// <exception cref="ArgumentException">Thrown when the type doesn't implement IErrorMapper</exception>
    /// <remarks>
    /// The specified mapper type must implement <see cref="IErrorMapper"/> and have
    /// a constructor compatible with dependency injection.
    /// </remarks>
    public ErrorHandlingOptions AddMapper(Type mapperType)
    {
        if (!typeof(IErrorMapper).IsAssignableFrom(mapperType))
        {
            throw new ArgumentException(
                $"Type {mapperType.Name} must implement {nameof(IErrorMapper)}",
                nameof(mapperType)
            );
        }

        CustomMappers.Add(mapperType);
        return this;
    }
}
