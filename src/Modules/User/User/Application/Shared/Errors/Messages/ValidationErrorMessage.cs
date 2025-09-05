namespace _116.User.Application.Shared.Errors.Messages;

/// <summary>
/// Provides validation-related error messages for the <c>User</c> domain.
/// These messages describe failures due to invalid input or format requirements.
/// </summary>
public static class ValidationErrorMessage
{
    /// <summary>
    /// Gets an error message for when an email has an invalid format.
    /// </summary>
    /// <param name="email">The invalid email address.</param>
    /// <returns>
    /// A formatted error message indicating that the provided email format is invalid.
    /// </returns>
    public static string InvalidEmailFormat(string email)
    {
        return $"Invalid email format: {email}";
    }

    /// <summary>
    /// Error message indicating that a password does not meet security requirements.
    /// </summary>
    public static string InvalidPasswordFormat()
    {
        return "Password does not meet security requirements";
    }

    /// <summary>
    /// Gets an error message for when a username does not meet required rules or format.
    /// </summary>
    /// <param name="userName">The invalid username.</param>
    /// <returns>
    /// A formatted error message indicating that the specified username is invalid.
    /// </returns>
    public static string InvalidUsernameFormat(string userName)
    {
        return $"Username '{userName}' does not meet requirements";
    }

    /// <summary>
    /// Error message indicating that permission resource is required.
    /// </summary>
    public static string PermissionResourceRequired()
    {
        return "Permission resource is required";
    }

    /// <summary>
    /// Error message indicating that permission action is required.
    /// </summary>
    public static string PermissionActionRequired()
    {
        return "Permission action is required";
    }

    /// <summary>
    /// Error message indicating that permission description is required.
    /// </summary>
    public static string PermissionDescriptionRequired()
    {
        return "Permission description is required";
    }

    /// <summary>
    /// Error message indicating that role name is required.
    /// </summary>
    public static string RoleNameRequired()
    {
        return "Role name is required";
    }

    /// <summary>
    /// Error message indicating that role description is required.
    /// </summary>
    public static string RoleDescriptionRequired()
    {
        return "Role description is required";
    }
}
