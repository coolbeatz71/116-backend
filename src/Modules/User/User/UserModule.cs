using System.Text;
using _116.Core.Application.Configurations;
using _116.Core.Infrastructure;
using _116.User.Application.Authorizations.Policies;
using _116.User.Application.Authorizations.Requirements;
using _116.User.Application.Services;
using _116.User.Domain.Enums;
using _116.User.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace _116.User;

/// <summary>
/// Provides extension methods to register and configure the User module's services and middleware.
/// </summary>
public static class UserModule
{
    /// <summary>
    /// Gets the shared module configuration options for the User module.
    /// </summary>
    private static ModuleOptions<UserDbContext> GetModuleOptions() => new()
    {
        ModuleName = "User",
        SchemaName = "user",
        EnableMigrations = true,
        EnableSeeding = true
    };

    /// <summary>
    /// Adds the User module's services to the dependency injection container.
    /// </summary>
    /// <param name="services">The service collection to register services into.</param>
    /// <param name="configuration">The application configuration instance.</param>
    /// <returns>The updated <see cref="IServiceCollection"/> for chaining.</returns>
    /// <remarks>
    /// Registers database context with interceptors, authentication services, JWT configuration,
    /// and authorization policies for user management.
    /// </remarks>
    /// <example>
    /// <code>
    /// builder.Services.AddUserModule(builder.Configuration);
    /// </code>
    /// </example>
    public static IServiceCollection AddUserModule(this IServiceCollection services, IConfiguration configuration)
    {
        // Add services to the container.
        // Api Endpoint services.
        // Application UseCase services.
        // DataSource - Infrastructure services.

        // Register the database with base module infrastructure
        services.AddModuleDatabase(configuration, GetModuleOptions());

        // Register user management services.
        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<IPasswordService, PasswordService>();

        // Register data seeder for initial user data population.
        // services.AddScoped<IDataSeeder, UserDataSeeder>();

        // Configure JWT Authentication.
        var (secret, issuer, audience, _) = AppEnvironment.Jwt();
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = issuer,
                ValidAudience = audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret!)),
                ClockSkew = TimeSpan.Zero
            };
        });

        // Configure Authorization Policies.
        // TODO: should replace this with a more robust setup
        // services.AddAuthorizationBuilder()
        //     .AddPolicy("RequireVerifiedUser", policy => policy.RequireClaim("is_verified", "true"))
        //     .AddPolicy("RequireActiveUser", policy => policy.RequireClaim("is_active", "true"))
        //     .AddPolicy("RequireLoggedInUser", policy => policy.RequireClaim("is_logged_in", "true"))
        //     .AddPolicy("LocalAuthOnly", policy => policy.RequireClaim("auth_provider", nameof(AuthProvider.Local)))
        //     .AddPolicy("ExternalAuthOnly", policy => policy.RequireAssertion(context =>
        //         {
        //             string? authProvider = context.User.FindFirst("auth_provider")?.Value;
        //             return authProvider != nameof(AuthProvider.Local);
        //         })
        //     );
        AuthorizationBuilder authBuilder = services.AddAuthorizationBuilder();

        var policyUserRoleMap = new Dictionary<string, string[]>
        {
            { UserRolePolicies.RequireAdminOnly, [nameof(CoreUserRole.Admin)] },
            { UserRolePolicies.RequireSuperAdminOnly, [nameof(CoreUserRole.SuperAdmin)] }
        };

        foreach (var (policyName, roles) in policyUserRoleMap)
        {
            authBuilder.AddPolicy(policyName, policy =>
                policy.Requirements.Add(new UserRoleRequirement(roles))
            );
        }

        return services;
    }

    /// <summary>
    /// Configures the User module's middleware in the application pipeline.
    /// </summary>
    /// <param name="app">The application builder.</param>
    /// <returns>The updated <see cref="IApplicationBuilder"/> for chaining.</returns>
    /// <remarks>
    /// Applies pending EF Core migrations and executes the data seeder for user management.
    /// </remarks>
    /// <example>
    /// <code>
    /// app.UseUserModule();
    /// </code>
    /// </example>
    public static IApplicationBuilder UseUserModule(this IApplicationBuilder app)
    {
        // Configure Http request pipeline.
        // Use Api endpoint services.
        // Use application UseCase services.
        // Use DataSource - Infrastructure services.
        app.UseModuleDatabase(GetModuleOptions());

        return app;
    }
}
