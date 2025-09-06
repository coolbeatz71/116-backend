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
    public async Task<OtpEntity?> GetLatestValidOtpAsync(
        Guid userId,
        OtpPurpose purpose,
        CancellationToken cancellationToken = default
    )
    {
        return await context.Otps
            .Where(o => o.UserId == userId
                        && o.Purpose == purpose
                        && o.IsUsed == false
                        && o.ExpiresAt > DateTime.UtcNow
            )
            .OrderByDescending(o => o.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<OtpEntity> ValidateOtpAsync(
        Guid userId,
        string code,
        OtpPurpose purpose,
        CancellationToken cancellationToken = default
    )
    {
        // Try to find the OTP with the provided code
        OtpEntity? matchingOtp = await context.Otps
            .Where(o => o.UserId == userId
                        && o.Code == code
                        && o.Purpose == purpose
                        && o.IsUsed == false
            )
            .OrderByDescending(o => o.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);

        if (matchingOtp != null)
        {
            // First check if the otp is not expired
            if (matchingOtp.IsExpired())
            {
                throw UserErrors.OtpExpired();
            }

            // Then check if the max attempts are not reached
            if (matchingOtp.HasMaxAttemptsReached())
            {
                throw UserErrors.MaxOtpAttemptsReached();
            }

            // Then check if the otp is valid for the user
            if (matchingOtp.IsValid())
            {
                return matchingOtp;
            }

            // Now increment the attempt count
            matchingOtp.IncrementAttemptCount();
            await UpdateAsync(matchingOtp, cancellationToken);

            if (matchingOtp.HasMaxAttemptsReached()) throw UserErrors.MaxOtpAttemptsReached();

            throw UserErrors.InvalidOtpCode();
        }

        // No matching OTP found — check the latest valid OTP for this purpose
        OtpEntity? latestOtp = await GetLatestValidOtpAsync(userId, purpose, cancellationToken);

        if (latestOtp == null)
        {
            throw UserErrors.NoValidOtpFound();
        }

        if (latestOtp.HasMaxAttemptsReached())
        {
            throw UserErrors.MaxOtpAttemptsReached();
        }

        if (latestOtp.IsExpired())
        {
            throw UserErrors.OtpExpired();
        }

        // Increment attempts for wrong code
        latestOtp.IncrementAttemptCount();
        await UpdateAsync(latestOtp, cancellationToken);

        if (latestOtp.HasMaxAttemptsReached())
        {
            throw UserErrors.MaxOtpAttemptsReached();
        }

        throw UserErrors.InvalidOtpCode();
    }


    /// <inheritdoc />
    public async Task InvalidateExistingOtpsAsync(
        Guid userId,
        OtpPurpose purpose,
        CancellationToken cancellationToken = default
    )
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
