using _116.BuildingBlocks.Constants;
using _116.User.Application.Authorizations.Policies;
using _116.User.Domain.Enums;

namespace _116.User.Application.Authorizations.Configuration;

/// <summary>
/// Centralized configuration for authorization policies in the User module.
/// </summary>
/// <remarks>
/// This class follows the configuration-as-code pattern, providing a single source of truth
/// for all authorization policy definitions. It supports the modular monolithic architecture
/// by encapsulating policy configuration within the User module while maintaining clean separation
/// between policy definition and implementation logic.
/// </remarks>
public static class AuthorizationPolicyConfiguration
{
    /// <summary>
    /// Gets the complete authorization policy configuration for the User module.
    /// </summary>
    /// <returns>A configuration object containing all policy definitions</returns>
    /// <remarks>
    /// This method provides a centralized way to access all authorization policy configurations.
    /// The returned configuration includes both account status policies and user role policies,
    /// making it easy to understand and modify the complete authorization setup.
    /// </remarks>
    public static AuthorizationConfiguration GetConfiguration()
    {
        return new AuthorizationConfiguration
        {
            AccountStatusPolicies = GetAccountStatusPolicies(),
            UserRolePolicies = GetUserRolePolicies()
        };
    }

    /// <summary>
    /// Defines account status policies with their corresponding claim requirements.
    /// </summary>
    /// <returns>Dictionary mapping policy names to claim type and value pairs</returns>
    /// <remarks>
    /// These policies control access based on user account status claims in JWT tokens.
    /// Each policy maps to a specific claim type and expected value for authorization.
    /// </remarks>
    private static Dictionary<string, (string ClaimType, string ClaimValue)> GetAccountStatusPolicies()
    {
        return new Dictionary<string, (string ClaimType, string ClaimValue)>
        {
            { AccountStatusPolicies.RequireVerifiedUser, (JwtClaimsConstants.IsVerified, "true") },
            { AccountStatusPolicies.RequireActiveUser, (JwtClaimsConstants.IsActive, "true") },
            { AccountStatusPolicies.RequireLoggedInUser, (JwtClaimsConstants.IsLoggedIn, "true") }
        };
    }

    /// <summary>
    /// Defines user role policies with their corresponding role requirements.
    /// </summary>
    /// <returns>Dictionary mapping policy names to arrays of allowed roles</returns>
    /// <remarks>
    /// These policies control access based on user roles. Each policy can specify
    /// multiple allowed roles, providing flexibility in role-based authorization.
    /// Uses core system roles defined in <see cref="CoreUserRole"/> enumeration.
    /// </remarks>
    private static Dictionary<string, string[]> GetUserRolePolicies()
    {
        return new Dictionary<string, string[]>
        {
            { UserRolePolicies.RequireAdminOnly, [nameof(CoreUserRole.Admin)] },
            { UserRolePolicies.RequireSuperAdminOnly, [nameof(CoreUserRole.SuperAdmin)] }
        };
    }
}

/// <summary>
/// Represents the complete authorization configuration for the User module.
/// </summary>
/// <remarks>
/// This record encapsulates all authorization policy configurations in a structured format,
/// promoting type safety and immutability in the configuration system.
/// </remarks>
public sealed record AuthorizationConfiguration
{
    /// <summary>
    /// Gets or sets the account status policy configurations.
    /// </summary>
    /// <remarks>
    /// Contains mappings between policy names and their claim type/value requirements.
    /// </remarks>
    public required Dictionary<string, (string ClaimType, string ClaimValue)> AccountStatusPolicies { get; init; }

    /// <summary>
    /// Gets or sets the user role policy configurations.
    /// </summary>
    /// <remarks>
    /// Contains mappings between policy names and their allowed role arrays.
    /// </remarks>
    public required Dictionary<string, string[]> UserRolePolicies { get; init; }
}
