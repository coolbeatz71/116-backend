using System.ComponentModel.DataAnnotations;
using _116.Core.Domain;

namespace _116.User.Domain.Entities;

/// <summary>
/// Represents a role that can be assigned to users and associated with permissions.
/// </summary>
public class RoleEntity : Aggregate<Guid>
{
    /// <summary>
    /// Name of the role (e.g., "Admin", "Editor").
    /// </summary>
    [MaxLength(20)]
    public string Name { get; private set; } = null!;

    /// <summary>
    /// Human-readable description of the role's purpose or scope.
    /// </summary>
    [MaxLength(300)]
    public string Description { get; private set; } = null!;

    /// <summary>
    /// Navigation property:
    /// Collection of user-role associations linking users to this role.
    /// </summary>
    public ICollection<UserRoleEntity> UserRoles { get; private set; } = new List<UserRoleEntity>();

    /// <summary>
    /// Navigation property:
    /// Collection of role-permission associations linking this role to its permissions.
    /// </summary>
    public ICollection<RolePermissionEntity> RolePermissions { get; private set; } = new List<RolePermissionEntity>();
}
