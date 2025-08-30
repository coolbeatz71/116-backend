namespace _116.BuildingBlocks.Constants;

/// <summary>
/// Constants for exception reasons and error codes used throughout the application.
/// </summary>
public static class ExceptionConstants
{
    /// <summary>
    /// Authentication error reasons.
    /// </summary>
    public static class Authentication
    {
        /// <summary>
        /// User lacks sufficient role privileges for the requested operation.
        /// </summary>
        public const string InsufficientRole = "InsufficientRole";

        /// <summary>
        /// Invalid password provided during authentication.
        /// </summary>
        public const string InvalidPassword = "InvalidPassword";

        /// <summary>
        /// Invalid credentials provided (generic authentication failure).
        /// </summary>
        public const string InvalidCredentials = "InvalidCredentials";
    }

    /// <summary>
    /// Authorization error reasons.
    /// </summary>
    public static class Authorization
    {
        /// <summary>
        /// User account is inactive or disabled.
        /// </summary>
        public const string ActiveAccount = "ActiveAccount";

        /// <summary>
        /// User account is not verified.
        /// </summary>
        public const string UnverifiedAccount = "UnverifiedAccount";

        /// <summary>
        /// User account is locked due to security reasons.
        /// </summary>
        public const string AccountLocked = "AccountLocked";

        /// <summary>
        /// User lacks required permissions for the resource.
        /// </summary>
        public const string InsufficientPermissions = "InsufficientPermissions";
    }

    /// <summary>
    /// Authentication schemes used in the application.
    /// </summary>
    public static class Schemes
    {
        /// <summary>
        /// JWT Bearer token authentication scheme.
        /// </summary>
        public const string JwtBearer = "JwtBearer";

        /// <summary>
        /// Local email/password authentication scheme.
        /// </summary>
        public const string Local = "Local";

        /// <summary>
        /// Admin authentication scheme for administrative operations.
        /// </summary>
        public const string Admin = "Admin";
    }
}
