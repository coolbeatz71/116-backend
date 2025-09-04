namespace _116.BuildingBlocks.Constants;

/// <summary>
/// Defines custom claim type constants for JWT tokens.
/// </summary>
public static class JwtClaimsConstants
{
    /// <summary>
    /// Claim type for the authentication provider.
    /// </summary>
    public const string AuthProvider = "auth_provider";

    /// <summary>
    /// Claim type for user roles.
    /// </summary>
    public const string Roles = "roles";

    /// <summary>
    /// Claim type for user permissions.
    /// </summary>
    public const string Permissions = "permissions";

    /// <summary>
    /// Claim type for user verification status.
    /// </summary>
    public const string IsVerified = "is_verified";

    /// <summary>
    /// Claim type for user active status.
    /// </summary>
    public const string IsActive = "is_active";

    /// <summary>
    /// Claim type for user login status.
    /// </summary>
    public const string IsLoggedIn = "is_logged_in";

}
