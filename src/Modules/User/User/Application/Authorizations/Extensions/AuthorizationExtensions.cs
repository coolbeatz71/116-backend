using _116.User.Application.Authorizations.Configuration;
using _116.User.Application.Authorizations.Handlers;
using _116.User.Application.Authorizations.Requirements;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace _116.User.Application.Authorizations.Extensions;

/// <summary>
/// Provides extension methods for configuring authorization policies and requirements.
/// </summary>
/// <remarks>
/// Centralizes authorization configuration to promote consistency and reusability across modules.
/// Follows the modular monolithic architecture pattern by encapsulating authorization concerns
/// within the User module while providing extensibility for other modules.
/// </remarks>
public static class AuthorizationExtensions
{
    /// <summary>
    /// Configures authorization policies and handlers for the User module.
    /// </summary>
    /// <param name="services">The service collection to add authorization services to</param>
    /// <returns>The service collection for method chaining</returns>
    /// <remarks>
    /// This method encapsulates the complete authorization setup including:
    /// <list type="bullet">
    /// <item>Registration of authorization requirement handlers</item>
    /// <item>Configuration of account status policies</item>
    /// <item>Configuration of user role policies</item>
    /// </list>
    ///
    /// The configuration is data-driven and easily extensible for new policy requirements.
    /// </remarks>
    public static IServiceCollection AddUserModuleAuthorization(this IServiceCollection services)
    {
        // Register authorization requirement handlers
        services.AddScoped<IAuthorizationHandler, UserRoleRequirementHandler>();
        services.AddScoped<IAuthorizationHandler, AccountStatusRequirementHandler>();

        // Configure authorization policies
        services.ConfigureAuthorizationPolicies();

        return services;
    }

    /// <summary>
    /// Configures authorization policies using a centralized, data-driven approach.
    /// </summary>
    /// <param name="services">The service collection to add policies to</param>
    /// <returns>The service collection for method chaining</returns>
    private static IServiceCollection ConfigureAuthorizationPolicies(this IServiceCollection services)
    {
        AuthorizationBuilder authBuilder = services.AddAuthorizationBuilder();

        // Configure policies using centralized configuration
        AuthorizationConfiguration policyConfiguration = AuthorizationPolicyConfiguration.GetConfiguration();

        authBuilder.ConfigureAccountStatusPolicies(policyConfiguration.AccountStatusPolicies);
        authBuilder.ConfigureUserRolePolicies(policyConfiguration.UserRolePolicies);

        return services;
    }

    /// <summary>
    /// Configures account status policies using requirements.
    /// </summary>
    /// <param name="authBuilder">The authorization builder</param>
    /// <param name="policies">Dictionary of policy configurations</param>
    /// <returns>The authorization builder for method chaining</returns>
    private static AuthorizationBuilder ConfigureAccountStatusPolicies(
        this AuthorizationBuilder authBuilder,
        Dictionary<string, (string ClaimType, string ClaimValue)> policies
    )
    {
        foreach (var (policyName, (claimType, claimValue)) in policies)
        {
            authBuilder.AddPolicy(policyName, policy =>
                policy.Requirements.Add(new AccountStatusRequirement(claimType, claimValue))
            );
        }

        return authBuilder;
    }

    /// <summary>
    /// Configures user role policies using requirements.
    /// </summary>
    /// <param name="authBuilder">The authorization builder</param>
    /// <param name="policies">Dictionary of policy configurations</param>
    /// <returns>The authorization builder for method chaining</returns>
    private static AuthorizationBuilder ConfigureUserRolePolicies(
        this AuthorizationBuilder authBuilder,
        Dictionary<string, string[]> policies
    )
    {
        foreach (var (policyName, roles) in policies)
        {
            authBuilder.AddPolicy(policyName, policy =>
                policy.Requirements.Add(new UserRoleRequirement(roles))
            );
        }

        return authBuilder;
    }
}
