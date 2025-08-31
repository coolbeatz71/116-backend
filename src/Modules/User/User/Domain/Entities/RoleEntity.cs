using System.ComponentModel.DataAnnotations;
using _116.BuildingBlocks.Constants;
using _116.Shared.Domain;

namespace _116.User.Domain.Entities;

/// <summary>
/// Represents a role that can be assigned to users and associated with permissions.
/// </summary>
public class RoleEntity : Aggregate<Guid>
{
    /// <summary>
    /// Name of the role (e.g., "Admin", "Editor").
    /// </summary>
    [MaxLength(RoleConstants.MaxRoleNameLength)]
    public string Name { get; private set; } = null!;

    /// <summary>
    /// Human-readable description of the role's purpose or scope.
    /// </summary>
    [MaxLength(RoleConstants.MaxRoleDescriptionLength)]
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

    /// <summary>
    /// Creates a new role entity.
    /// </summary>
    /// <param name="id">The unique identifier of the role.</param>
    /// <param name="name">The name of the role.</param>
    /// <param name="description">The description of the role's purpose.</param>
    /// <returns>A new <see cref="RoleEntity"/> instance.</returns>
    /// <exception cref="ArgumentException">Thrown when name or description are empty.</exception>
    public static RoleEntity Create(Guid id, string name, string description)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Role name is required", nameof(name));
        }

        if (string.IsNullOrWhiteSpace(description))
        {
            throw new ArgumentException("Role description is required", nameof(description));
        }

        return new RoleEntity
        {
            Id = id,
            Name = name,
            Description = description
        };
    }
}
