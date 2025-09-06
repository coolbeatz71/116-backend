using _116.Shared.Contracts.Application.CQRS;

namespace _116.User.Application.Public.UseCases.Commands.VerifyOtp;

/// <summary>
/// Command for verifying an OTP code for user account verification.
/// </summary>
/// <param name="Email">The user's email address.</param>
/// <param name="Code">The OTP code to verify.</param>
/// <remarks>
/// This command is used to verify the OTP code sent to the user's email during registration.
/// Upon successful verification, the user's account will be marked as verified.
/// </remarks>
public record VerifyOtpCommand(
    string Email,
    string Code
) : ICommand<VerifyOtpResult>;

/// <summary>
/// Result of the <see cref="VerifyOtpCommand"/> containing verification status.
/// </summary>
/// <param name="IsSuccess">Indicates whether the OTP verification was successful.</param>
/// <remarks>
/// Contains the verification result and a user-friendly message explaining the outcome.
/// </remarks>
public record VerifyOtpResult(
    bool IsSuccess
);
