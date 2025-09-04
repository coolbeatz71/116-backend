namespace _116.User.Application.Constants;

/// <summary>
/// Error messages for the User domain.
/// Provides consistent, localization-ready error messages.
/// </summary>
public static class UserErrorMessages
{
    /// <summary>
    /// Conflict error messages.
    /// </summary>
    public static class Conflict
    {
        /// <summary>
        /// Gets an error message for when user email already exists.
        /// </summary>
        public static string EmailAlreadyExists(string email) =>
            $"User with email '{email}' already exists";

        /// <summary>
        /// Gets an error message for when username is already taken.
        /// </summary>
        public static string UsernameAlreadyExists(string username) =>
            $"Username '{username}' is already taken";

        /// <summary>
        /// Gets an error message for when the role already exists.
        /// </summary>
        public static string RoleAlreadyExists(string roleName) =>
            $"Role '{roleName}' already exists";
    }

    /// <summary>
    /// Authentication error messages.
    /// </summary>
    public static class Authentication
    {
        /// <summary>
        /// Generic invalid credentials message (security best practice).
        /// </summary>
        public const string InvalidCredentials = "Invalid email or password";

        /// <summary>
        /// Gets error message for admin privileges requirement.
        /// </summary>
        public static string AdminPrivilegesRequired(string operation) =>
            $"Administrative privileges are required for '{operation}'";
    }

    /// <summary>
    /// Authorization error messages.
    /// </summary>
    public static class Authorization
    {
        /// <summary>
        /// Gets an error message for an inactive account.
        /// </summary>
        public static string AccountInactive(string email) =>
            $"Account for '{email}' is inactive";

        /// <summary>
        /// Gets an error message for an unverified account.
        /// </summary>
        public static string AccountNotVerified(string email) =>
            $"Account for '{email}' is not verified";

        /// <summary>
        /// Gets an error message for a locked account.
        /// </summary>
        public static string AccountLocked(string email) =>
            $"Account for '{email}' is locked";
    }

    /// <summary>
    /// Validation error messages.
    /// </summary>
    public static class Validation
    {
        /// <summary>
        /// Gets an error message for invalid email format.
        /// </summary>
        public static string InvalidEmailFormat(string email) =>
            $"Invalid email format: {email}";

        /// <summary>
        /// Error message for invalid password format.
        /// </summary>
        public const string InvalidPasswordFormat = "Password does not meet security requirements";

        /// <summary>
        /// Gets error message for invalid username format.
        /// </summary>
        public static string InvalidUsernameFormat(string username) =>
            $"Username '{username}' does not meet requirements";
    }
}
