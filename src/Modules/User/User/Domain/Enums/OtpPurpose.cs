namespace _116.User.Domain.Enums;

/// <summary>
/// Defines the different purposes for which an OTP can be used.
/// </summary>
public enum OtpPurpose
{
    /// <summary>
    /// OTP used for email verification during account registration.
    /// </summary>
    EmailVerification,

    /// <summary>
    /// OTP used for password reset requests.
    /// </summary>
    PasswordReset,

    /// <summary>
    /// OTP used for two-factor authentication.
    /// </summary>
    TwoFactorAuthentication,

    /// <summary>
    /// OTP used for account recovery.
    /// </summary>
    AccountRecovery
}
