using _116.BuildingBlocks.Constants;
using _116.Shared.Application.Exceptions;
using _116.Shared.Infrastructure.Extensions;
using _116.User.Application.Shared.Repositories;
using _116.User.Domain.Entities;
using _116.User.Domain.Enums;
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
    public async Task<UserEntity> GetActiveAdminUserWithRolesAndPermissionsAsync(Email email, CancellationToken cancellationToken = default)
    {
        // First, get the user without filtering by IsActive to provide specific error messages
        UserEntity user = await context.Users
            .Where(u => u.Email == email.Value)
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                    .ThenInclude(r => r.RolePermissions)
                        .ThenInclude(rp => rp.Permission)
            .FirstDefaultOrThrowAsync(
                keyName: "email",
                keyValue: email.Value,
                cancellationToken: cancellationToken
            );

        // Check if the account is active
        if (!user.IsActive)
        {
            throw new AuthorizationException(
                "Access to the account forbidden.",
                ExceptionConstants.Authorization.ActiveAccount
            );
        }

        // Validate admin privileges
        bool hasAdminRole = user.UserRoles
            .Any(ur => ur.Role.Name is nameof(CoreUserRole.Admin) or nameof(CoreUserRole.SuperAdmin));

        if (!hasAdminRole)
        {
            throw new AuthenticationException(
                "Invalid credentials or insufficient privileges.",
                ExceptionConstants.Authentication.InsufficientRole
            );
        }

        return user;
    }

    /// <inheritdoc />
    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await context.SaveChangesAsync(cancellationToken);
    }
}
