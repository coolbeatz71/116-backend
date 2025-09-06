using FluentValidation;

namespace _116.User.Application.Public.UseCases.Commands.VerifyOtp;

/// <summary>
/// Validator for the <see cref="VerifyOtpCommand"/> ensuring proper OTP verification data format.
/// </summary>
/// <remarks>
/// Validates email and OTP code according to format requirements:
/// - Email: Valid email format
/// - Code: 6-digit numeric code
/// </remarks>
public class VerifyOtpValidator : AbstractValidator<VerifyOtpCommand>
{
    /// <summary>
    /// Configure validation rules for OTP verification.
    /// </summary>
    public VerifyOtpValidator()
    {
        // Email validation
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format");

        // OTP code validation
        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("Verification code is required");
    }
}
