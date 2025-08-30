using _116.Shared.Infrastructure.Extensions;
using _116.User.Application.Shared.Repositories;
using _116.User.Domain.Entities;
using _116.User.Domain.ValueObjects;
using _116.User.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace _116.User.Infrastructure.Repositories;

/// <summary>
/// Implementation of <see cref="IUserRepository"/> using Entity Framework Core.
/// </summary>
public class UserRepository(UserDbContext context) : IUserRepository
{
    /// <inheritdoc />
    public async Task<UserEntity> GetUserWithRolesOrThrowAsync(Email email, CancellationToken cancellationToken = default)
    {
        return await context.Users
            .Where(u => u.Email == email.Value)
            .Include(u => u.UserRoles).ThenInclude(ur => ur.Role)
            .FirstDefaultOrThrowAsync(
                keyName: "credentials",
                keyValue: email.Value,
                cancellationToken: cancellationToken
            );
    }

    /// <inheritdoc />
    public async Task<UserEntity?> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await context.Users.FindAsync([userId], cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> ExistsByEmailAsync(Email email, CancellationToken cancellationToken = default)
    {
        return await context.Users.AnyAsync(u => u.Email == email.Value, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> ExistsByUserNameAsync(string userName, CancellationToken cancellationToken = default)
    {
        return await context.Users.AnyAsync(u => u.UserName == userName, cancellationToken);
    }

    /// <inheritdoc />
    public async Task AddAsync(UserEntity user, CancellationToken cancellationToken = default)
    {
        await context.Users.AddAsync(user, cancellationToken);
    }

    /// <inheritdoc />
    public async Task UpdateAsync(UserEntity user, CancellationToken cancellationToken = default)
    {
        context.Users.Update(user);
        await context.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await context.SaveChangesAsync(cancellationToken);
    }
}
