using _116.Core.Application.ErrorHandling.Abstractions;
using _116.Core.Application.ErrorHandling.Mappers;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace _116.Core.Application.ErrorHandling.Extensions;

/// <summary>
/// Provides extension methods for configuring unified error handling in the application.
/// </summary>
/// <remarks>
/// These extensions simplify the registration and configuration of the unified error handling system,
/// allowing modules to easily integrate comprehensive error handling with minimal configuration.
/// The system uses the Strategy pattern with error mappers to handle different error types
/// while maintaining consistent response formatting across the entire application.
/// </remarks>
public static class ErrorHandlingExtensions
{
    /// <summary>
    /// Adds the unified error handling services to the dependency injection container.
    /// </summary>
    /// <param name="services">The service collection to add services to</param>
    /// <returns>The service collection for method chaining</returns>
    /// <remarks>
    /// This method registers all necessary parts for the unified error handling system:
    /// <list type="bullet">
    /// <item>Default error mappers for common exception types</item>
    /// <item>Authentication-specific error mapper for JWT and security errors</item>
    /// <item>The unified error handler that orchestrates all error processing</item>
    /// </list>
    ///
    /// The error mappers are registered with appropriate priorities to ensure specialized
    /// handling takes precedence over generic handling.
    /// </remarks>
    public static IServiceCollection AddErrorPipelineHandler(this IServiceCollection services)
    {
        // Register error mappers in order of priority
        services.AddSingleton<IErrorMapper, AuthenticationErrorMapper>();
        services.AddSingleton<IErrorMapper, ExceptionErrorMapper>();

        // Register the error pipeline handler
        services.AddSingleton<IErrorHandler, ErrorPipelineHandler>();

        // Register as exception handler for ASP.NET Core pipeline
        services.AddExceptionHandler<ErrorPipelineHandler>();

        return services;
    }

    /// <summary>
    /// Adds a custom error mapper to the error handling system.
    /// </summary>
    /// <typeparam name="TErrorMapper">The type of error mapper to add</typeparam>
    /// <param name="services">The service collection to add the mapper to</param>
    /// <returns>The service collection for method chaining</returns>
    /// <remarks>
    /// Use this method to add specialized error mappers for domain-specific error types.
    /// Custom mappers should implement <see cref="IErrorMapper"/> and define appropriate
    /// priority values to control the order of evaluation.
    /// </remarks>
    public static IServiceCollection AddErrorMapper<TErrorMapper>(this IServiceCollection services)
        where TErrorMapper : class, IErrorMapper
    {
        services.AddSingleton<IErrorMapper, TErrorMapper>();
        return services;
    }

    /// <summary>
    /// Configures the unified error handling middleware in the application pipeline.
    /// </summary>
    /// <param name="app">The application builder to configure</param>
    /// <returns>The application builder for method chaining</returns>
    /// <remarks>
    /// This method should be called early in the middleware pipeline to ensure all errors
    /// are properly handled. It configures the ASP.NET Core exception handling middleware
    /// to use the unified error handler for consistent error processing.
    ///
    /// Place this call after authentication/authorization middleware but before
    /// application-specific middleware to ensure security context is available
    /// during error processing.
    /// </remarks>
    public static IApplicationBuilder UseErrorPipelineHandler(this IApplicationBuilder app)
    {
        app.UseExceptionHandler(_ => { });
        return app;
    }

    /// <summary>
    /// Adds unified error handling with custom configuration options.
    /// </summary>
    /// <param name="services">The service collection to add services to</param>
    /// <param name="configureOptions">Action to configure error handling options</param>
    /// <returns>The service collection for method chaining</returns>
    /// <remarks>
    /// This overload allows for custom configuration of the error handling system,
    /// such as adding custom error mappers or configuring logging behavior.
    /// </remarks>
    public static IServiceCollection AddErrorPipelineHandler(
        this IServiceCollection services,
        Action<ErrorHandlingOptions> configureOptions
    )
    {
        var options = new ErrorHandlingOptions();
        configureOptions(options);

        // Add default services
        services.AddErrorPipelineHandler();

        // Add custom mappers if specified
        foreach (Type mapperType in options.CustomMappers)
        {
            services.AddSingleton(typeof(IErrorMapper), mapperType);
        }

        return services;
    }
}

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
