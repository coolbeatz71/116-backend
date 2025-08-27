namespace _116.BuildingBlocks.Constants;

/// <summary>
/// Contains constants related to user entity business rules and constraints.
/// </summary>
public static class UserConstants
{
    /// <summary>
    /// Maximum allowed length for username.
    /// </summary>
    public const int MaxUserNameLength = 20;

    /// <summary>
    /// Default verification status for new local authentication users.
    /// </summary>
    public const bool DefaultIsVerified = false;

    /// <summary>
    /// Default active status for new users.
    /// </summary>
    public const bool DefaultIsActive = true;

    /// <summary>
    /// Default login status for new users.
    /// </summary>
    public const bool DefaultIsLoggedIn = false;

    /// <summary>
    /// Verification status for external authentication users (pre-verified).
    /// </summary>
    public const bool ExternalAuthIsVerified = true;

    /// <summary>
    /// Active status when the user account is activated.
    /// </summary>
    public const bool ActivatedStatus = true;

    /// <summary>
    /// Active status when the user account is deactivated.
    /// </summary>
    public const bool DeactivatedStatus = false;

    /// <summary>
    /// Login status when the user is logged in.
    /// </summary>
    public const bool LoggedInStatus = true;

    /// <summary>
    /// Login status when the user is logged out.
    /// </summary>
    public const bool LoggedOutStatus = false;

    /// <summary>
    /// Verification status after email is updated (requires re-verification).
    /// </summary>
    public const bool EmailUpdatedVerificationStatus = false;
}
