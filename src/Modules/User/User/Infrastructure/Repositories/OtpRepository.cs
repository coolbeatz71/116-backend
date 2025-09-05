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
    public async Task<OtpEntity?> ValidateOtpAsync(Guid userId, string code, OtpPurpose purpose, CancellationToken cancellationToken = default)
    {
        return await context.Otps
            .Where(o => o.UserId == userId
                        && o.Code == code
                        && o.Purpose == purpose
                        && !o.IsUsed
                        && o.ExpiresAt > DateTime.UtcNow
                        && o.AttemptCount < BuildingBlocks.Constants.UserConstants.MaxOtpAttempts)
            .OrderByDescending(o => o.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task InvalidateExistingOtpsAsync(Guid userId, OtpPurpose purpose, CancellationToken cancellationToken = default)
    {
        var existingOtps = await context.Otps
            .Where(o => o.UserId == userId && o.Purpose == purpose && !o.IsUsed)
            .ToListAsync(cancellationToken);

        foreach (var otp in existingOtps)
        {
            otp.MarkAsUsed();
        }
    }

    /// <inheritdoc />
    public async Task<int> CleanupExpiredOtpsAsync(CancellationToken cancellationToken = default)
    {
        var expiredOtps = await context.Otps
            .Where(o => o.ExpiresAt <= DateTime.UtcNow)
            .ToListAsync(cancellationToken);

        context.Otps.RemoveRange(expiredOtps);
        return expiredOtps.Count;
    }

    /// <inheritdoc />
    public async Task UpdateAsync(OtpEntity otp, CancellationToken cancellationToken = default)
    {
        context.Otps.Update(otp);
    }

    /// <inheritdoc />
    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await context.SaveChangesAsync(cancellationToken);
    }
}
