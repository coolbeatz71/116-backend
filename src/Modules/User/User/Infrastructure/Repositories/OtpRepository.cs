using _116.User.Application.Shared.Errors;
using _116.User.Application.Shared.Repositories;
using _116.User.Domain.Entities;
using _116.User.Domain.Enums;
using _116.User.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace _116.User.Infrastructure.Repositories;

/// <summary>
/// Implementation of <see cref="IOtpRepository"/> using Entity Framework Core.
/// </summary>
public class OtpRepository(UserDbContext context) : IOtpRepository
{
    /// <inheritdoc />
    public async Task AddAsync(OtpEntity otp, CancellationToken cancellationToken = default)
    {
        await context.Otps.AddAsync(otp, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<OtpEntity?> GetLatestValidOtpAsync(Guid userId, OtpPurpose purpose, CancellationToken cancellationToken = default)
    {
        return await context.Otps
            .Where(o => o.UserId == userId
                        && o.Purpose == purpose
                        && !o.IsUsed
                        && o.ExpiresAt > DateTime.UtcNow
                        && o.AttemptCount < BuildingBlocks.Constants.UserConstants.MaxOtpAttempts)
            .OrderByDescending(o => o.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<OtpEntity> ValidateOtpAsync(Guid userId, string code, OtpPurpose purpose, CancellationToken cancellationToken = default)
    {
        // First, try to find the exact OTP with the provided code
        OtpEntity? matchingOtp = await context.Otps
            .Where(o => o.UserId == userId
                        && o.Code == code
                        && o.Purpose == purpose
                        && !o.IsUsed
            )
            .OrderByDescending(o => o.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);

        if (matchingOtp != null && matchingOtp.IsValid())
        {
            return matchingOtp;
        }

        // If no matching OTP found, check for any valid OTP to provide the specific error
        OtpEntity? latestOtp = await GetLatestValidOtpAsync(userId, purpose, cancellationToken);

        if (latestOtp == null)
        {
            throw UserErrors.NoValidOtpFound();
        }

        // Increment attempt count for failed verification
        latestOtp.IncrementAttemptCount();
        await UpdateAsync(latestOtp, cancellationToken);

        // Check specific failure reasons
        if (latestOtp.HasMaxAttemptsReached())
        {
            throw UserErrors.MaxOtpAttemptsReached();
        }

        if (latestOtp.IsExpired())
        {
            throw UserErrors.OtpExpired();
        }

        // Invalid code
        throw UserErrors.InvalidOtpCode();
    }

    /// <inheritdoc />
    public async Task InvalidateExistingOtpsAsync(Guid userId, OtpPurpose purpose, CancellationToken cancellationToken = default)
    {
        List<OtpEntity> expiredOtpList = await context.Otps
            .Where(o => o.UserId == userId && o.Purpose == purpose && !o.IsUsed)
            .ToListAsync(cancellationToken);

        foreach (OtpEntity otp in expiredOtpList)
        {
            otp.MarkAsUsed();
        }
    }

    /// <inheritdoc />
    public async Task<int> CleanupExpiredOtpsAsync(CancellationToken cancellationToken = default)
    {
        List<OtpEntity> expiredOtpList = await context.Otps
            .Where(o => o.ExpiresAt <= DateTime.UtcNow)
            .ToListAsync(cancellationToken);

        context.Otps.RemoveRange(expiredOtpList);
        return expiredOtpList.Count;
    }

    /// <inheritdoc />
    public async Task UpdateAsync(OtpEntity otp, CancellationToken cancellationToken = default)
    {
        context.Otps.Update(otp);
        await context.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await context.SaveChangesAsync(cancellationToken);
    }
}
