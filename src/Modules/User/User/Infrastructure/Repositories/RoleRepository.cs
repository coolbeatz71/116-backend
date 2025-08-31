using _116.User.Application.Shared.Repositories;
using _116.User.Domain.DTOs;
using _116.User.Domain.Entities;
using _116.User.Infrastructure.Persistence;
using Mapster;

namespace _116.User.Infrastructure.Repositories;

/// <summary>
/// Implementation of <see cref="IRoleRepository"/> using Entity Framework Core.
/// </summary>
public class RoleRepository(UserDbContext context) : IRoleRepository
{
    /// <inheritdoc />
    public IReadOnlyCollection<RoleDto> GetUserRoles(ICollection<UserRoleEntity> userRoles)
    {
        return userRoles
            .Select(ur => ur.Role.Adapt<RoleDto>())
            .ToList();
    }

    /// <inheritdoc />
    public IReadOnlyCollection<PermissionDto> GetUserPermissions(ICollection<UserRoleEntity> userRoles)
    {
        return userRoles
            .SelectMany(ur => ur.Role.RolePermissions)
            .Select(rp => rp.Permission.Adapt<PermissionDto>())
            .DistinctBy(p => p.Id)
            .ToList();
    }

    /// <inheritdoc />
    public (IReadOnlyCollection<RoleDto> Roles, IReadOnlyCollection<PermissionDto> Permissions) GetUserRolesAndPermissions(ICollection<UserRoleEntity> userRoles)
    {
        IReadOnlyCollection<RoleDto> roles = GetUserRoles(userRoles);
        IReadOnlyCollection<PermissionDto> permissions = GetUserPermissions(userRoles);

        return (roles, permissions);
    }
}
