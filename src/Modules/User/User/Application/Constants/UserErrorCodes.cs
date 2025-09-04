namespace _116.User.Application.Constants;

/// <summary>
/// Error codes specific to the User domain.
/// </summary>
public static class UserErrorCodes
{
    /// <summary>
    /// Conflict error codes for User domain.
    /// </summary>
    public static class Conflict
    {
        /// <summary>
        /// User with the same email already exists.
        /// </summary>
        public const string UserAlreadyExists = "USER_ALREADY_EXISTS";

        /// <summary>
        /// Username is already taken by another user.
        /// </summary>
        public const string UsernameAlreadyExists = "USERNAME_ALREADY_EXISTS";

        /// <summary>
        /// Role with the same name already exists.
        /// </summary>
        public const string RoleAlreadyExists = "ROLE_ALREADY_EXISTS";

        /// <summary>
        /// Permission with the same resource and action already exists.
        /// </summary>
        public const string PermissionAlreadyExists = "PERMISSION_ALREADY_EXISTS";
    }

    /// <summary>
    /// Validation error codes for User domain.
    /// </summary>
    public static class Validation
    {
        /// <summary>
        /// Email format is invalid.
        /// </summary>
        public const string InvalidEmailFormat = "INVALID_EMAIL_FORMAT";

        /// <summary>
        /// Password does not meet requirements.
        /// </summary>
        public const string InvalidPasswordFormat = "INVALID_PASSWORD_FORMAT";

        /// <summary>
        /// Username does not meet requirements.
        /// </summary>
        public const string InvalidUsernameFormat = "INVALID_USERNAME_FORMAT";

        /// <summary>
        /// Role name is invalid or empty.
        /// </summary>
        public const string InvalidRoleName = "INVALID_ROLE_NAME";
    }

    /// <summary>
    /// Authentication error codes for User domain.
    /// </summary>
    public static class Authentication
    {
        /// <summary>
        /// Invalid credentials provided during login.
        /// </summary>
        public const string InvalidCredentials = "INVALID_CREDENTIALS";

        /// <summary>
        /// Admin privileges required for this operation.
        /// </summary>
        public const string AdminPrivilegesRequired = "ADMIN_PRIVILEGES_REQUIRED";
    }
}
