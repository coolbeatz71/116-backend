using _116.Shared.Application.Exceptions.Abstractions;
using _116.Shared.Application.Exceptions.Handlers.Authentication;
using _116.Shared.Application.Exceptions.Handlers.Common;
using _116.Shared.Application.Exceptions.Handlers.Validation;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace _116.Shared.Application.Exceptions.Core;

/// <summary>
/// Service registration and middleware configuration for the unified error pipeline.
/// </summary>
public static class ExceptionHandlerExtension
{
    /// <summary>
    /// Adds the unified error handling services to the dependency injection container.
    /// </summary>
    /// <remarks>
    /// Registers single-responsibility handlers ordered by <see cref="IErrorHandler.Priority"/>.
    /// Adding a new handler is as simple as implementing <see cref="IErrorHandler"/> and registering it.
    /// </remarks>
    public static void AddExceptionPipelineHandler(this IServiceCollection services)
    {
        // Response writer
        services.AddSingleton<IErrorResponseWriter, DefaultErrorResponseWriter>();

        // Authentication & Authorization
        services.AddSingleton<IErrorHandler, JwtTokenExceptionHandler>();
        services.AddSingleton<IErrorHandler, AuthenticationExceptionHandler>();
        services.AddSingleton<IErrorHandler, AuthorizationExceptionHandler>();

        // Validation
        services.AddSingleton<IErrorHandler, ValidationExceptionHandler>();

        // Common-known exceptions
        services.AddSingleton<IErrorHandler, CommonExceptionHandler>();

        // Fallback (must be present)
        services.AddSingleton<IErrorHandler, FallbackExceptionHandler>();

        // Register exception handler into ASP.NET Core pipeline
        services.AddExceptionHandler<ExceptionPipelineHandler>();
    }

    /// <summary>
    /// Configures the unified error handling middleware in the application pipeline.
    /// </summary>
    /// <remarks>
    /// Place this early, after authentication/authorization middleware, so the security context is available.
    /// </remarks>
    public static void UseExceptionPipelineHandler(this IApplicationBuilder app)
    {
        app.UseExceptionHandler(_ => { });
    }
}
