using _116.BuildingBlocks.Constants;
using _116.User.Domain.Entities;
using _116.User.Domain.Enums;

namespace _116.User.Application.Shared.Services;

/// <summary>
/// Implementation of <see cref="IOtpService"/> for OTP generation and management operations.
/// </summary>
public class OtpService : IOtpService
{
    private readonly Random _random = new();

    /// <inheritdoc />
    public string GenerateOtpCode()
    {
        // Generate a random numeric OTP code
        var code = _random.Next(0, (int)Math.Pow(10, UserConstants.OtpCodeLength))
            .ToString($"D{UserConstants.OtpCodeLength}");

        return code;
    }

    /// <inheritdoc />
    public OtpEntity CreateOtp(Guid userId, OtpPurpose purpose)
    {
        string code = GenerateOtpCode();
        DateTime expiresAt = CalculateExpirationTime();

        return OtpEntity.Create(
            id: Guid.NewGuid(),
            userId: userId,
            code: code,
            purpose: purpose,
            expiresAt: expiresAt
        );
    }

    /// <inheritdoc />
    public DateTime CalculateExpirationTime()
    {
        return DateTime.UtcNow.AddMinutes(UserConstants.OtpExpirationMinutes);
    }
}
