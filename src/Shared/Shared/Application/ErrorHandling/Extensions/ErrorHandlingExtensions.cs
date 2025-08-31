using _116.Shared.Application.ErrorHandling.Mappers;
using _116.Shared.Application.ErrorHandling.Mappers.Contracts;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace _116.Shared.Application.ErrorHandling.Extensions;

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
    public static void AddErrorPipelineHandler(this IServiceCollection services)
    {
        // Register error mappers in order of priority
        services.AddSingleton<IErrorMapper, AuthenticationErrorMapper>();
        services.AddSingleton<IErrorMapper, ExceptionErrorMapper>();

        // Register as exception handler for ASP.NET Core pipeline
        services.AddExceptionHandler<ErrorPipelineHandler>();
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
    public static void UseErrorPipelineHandler(this IApplicationBuilder app)
    {
        app.UseExceptionHandler(_ => { });
    }
}
