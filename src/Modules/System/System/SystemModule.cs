using _116.Shared.Application.Configurations;
using _116.Shared.Infrastructure;
using _116.System.Infrastructure.Persistence;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace _116.System;

/// <summary>
/// Provides extension methods to register and configure the System module's services and middleware.
/// </summary>
public static class SystemModule
{
    /// <summary>
    /// Gets the shared module configuration options for the System module.
    /// </summary>
    private static ModuleOptions<SystemDbContext> GetModuleOptions() => new()
    {
        ModuleName = "System",
        SchemaName = "system",
        EnableMigrations = true,
        EnableSeeding = false
    };

    /// <summary>
    /// Adds the System module's services to the dependency injection container.
    /// </summary>
    /// <param name="services">The service collection to register services into.</param>
    /// <param name="configuration">The application configuration instance.</param>
    /// <returns>The updated <see cref="IServiceCollection"/> for chaining.</returns>
    /// <remarks>
    /// Registers database context with interceptors for system management including file handling.
    /// </remarks>
    /// <example>
    /// <code>
    /// builder.Services.AddSystemModule(builder.Configuration);
    /// </code>
    /// </example>
    public static IServiceCollection AddSystemModule(this IServiceCollection services, IConfiguration configuration)
    {
        // Register the database with base module infrastructure
        services.AddModuleDatabase(configuration, GetModuleOptions());

        // Register system management services here when needed
        // services.AddScoped<IFileService, FileService>();

        return services;
    }

    /// <summary>
    /// Configures the System module's middleware in the application pipeline.
    /// </summary>
    /// <param name="app">The application builder.</param>
    /// <returns>The updated <see cref="IApplicationBuilder"/> for chaining.</returns>
    /// <remarks>
    /// Applies pending EF Core migrations for system management.
    /// </remarks>
    /// <example>
    /// <code>
    /// app.UseSystemModule();
    /// </code>
    /// </example>
    public static IApplicationBuilder UseSystemModule(this IApplicationBuilder app)
    {
        // Configure Http request pipeline.
        app.UseModuleDatabase(GetModuleOptions());

        return app;
    }
}
