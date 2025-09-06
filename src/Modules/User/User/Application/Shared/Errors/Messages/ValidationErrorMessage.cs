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

    /// <summary>
    /// Error message indicating that the user account is already verified.
    /// </summary>
    public static string AccountAlreadyVerified()
    {
        return "Account is already verified";
    }

    /// <summary>
    /// Error message indicating that no valid OTP was found for verification.
    /// </summary>
    public static string NoValidOtpFound()
    {
        return "No valid verification code found. Please request a new verification code";
    }

    /// <summary>
    /// Error message indicating that the provided OTP code is invalid.
    /// </summary>
    public static string InvalidOtpCode()
    {
        return "Invalid verification code. Please check and try again";
    }

    /// <summary>
    /// Error message indicating that the OTP has expired.
    /// </summary>
    public static string OtpExpired()
    {
        return "Verification code has expired. Please request a new verification code";
    }

    /// <summary>
    /// Error message indicating that maximum OTP verification attempts have been reached.
    /// </summary>
    public static string MaxOtpAttemptsReached()
    {
        return "Maximum verification attempts reached. Please request a new verification code";
    }
}
