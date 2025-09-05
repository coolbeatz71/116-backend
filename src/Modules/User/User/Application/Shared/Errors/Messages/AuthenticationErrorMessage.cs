namespace _116.User.Application.Shared.Errors.Messages;

/// <summary>
/// Provides authentication-related error messages for the <c>User</c> domain.
/// These messages describe authentication failures, such as invalid credentials
/// or missing administrative privileges.
/// </summary>
public static class AuthenticationErrorMessage
{
    /// <summary>
    /// Generic error message indicating that the provided login credentials
    /// are invalid. This message avoids leaking sensitive information
    /// for security reasons.
    /// </summary>
    public static string InvalidCredentials()
    {
        return "Invalid email or password";
    }
}
