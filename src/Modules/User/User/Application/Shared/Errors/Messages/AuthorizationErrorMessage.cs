namespace _116.User.Application.Shared.Errors.Messages;

/// <summary>
/// Provides authorization-related error messages for the <c>User</c> domain.
/// These messages describe failures related to account status or permission
/// restrictions.
/// </summary>
public static class AuthorizationErrorMessage
{
    /// <summary>
    /// Gets an error message for when an account is inactive.
    /// </summary>
    /// <param name="email">The email of the inactive account.</param>
    /// <returns>
    /// A formatted error message indicating that the specified account is inactive.
    /// </returns>
    public static string AccountInactive(string email)
    {
        return $"Account associated with '{email}' is inactive. Please contact support for assistance.";
    }

    /// <summary>
    /// Gets an error message for when an account is not verified.
    /// </summary>
    /// <param name="email">The email of the unverified account.</param>
    /// <returns>
    /// A formatted error message indicating that the specified account is not verified.
    /// </returns>
    public static string AccountNotVerified(string email)
    {
        return $"The account associated with '{email}' is not verified. Please complete the verification process to continue.";
    }
}
