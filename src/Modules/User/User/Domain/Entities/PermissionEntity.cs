using System.ComponentModel.DataAnnotations;
using _116.Core.Domain;

namespace _116.User.Domain.Entities;

/// <summary>
/// Represents a permission that defines an action allowed on a specific resource.
/// </summary>
/// <remarks>
/// Permissions are typically associated with roles through <see cref="RolePermissionEntity"/>.
/// </remarks>
public class PermissionEntity : Aggregate<Guid>
{
    /// <summary>
    /// The name or key of the resource (e.g., "user", "receipt", "article").
    /// </summary>
    [MaxLength(15)]
    public string Resource { get; private set; } = null!;

    /// <summary>
    /// The type of action allowed on the resource (e.g., "read", "create", "approve").
    /// </summary>
    [MaxLength(15)]
    public string Action { get; private set; } = null!;

    /// <summary>
    /// Human-readable description of the permission's purpose or scope.
    /// </summary>
    [MaxLength(300)]
    public string Description { get; private set; } = null!;

    /// <summary>
    /// Navigation property:
    /// Collection of role-permission associations linked to this permission.
    /// </summary>
    public ICollection<RolePermissionEntity> RolePermissions { get; private set; } = new List<RolePermissionEntity>();
}
