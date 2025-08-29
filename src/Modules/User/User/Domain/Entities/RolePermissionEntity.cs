using _116.Core.Domain;

namespace _116.User.Domain.Entities;

/// <summary>
/// Represents the many-to-many association between roles and permissions.
/// </summary>
public class RolePermissionEntity : Aggregate<Guid>
{
    /// <summary>
    /// Foreign key referencing the associated role.
    /// </summary>
    public Guid RoleId { get; private set; }

    /// <summary>
    /// Foreign key referencing the associated permission.
    /// </summary>
    public Guid PermissionId { get; private set; }

    /// <summary>
    /// Navigation property for the associated role.
    /// </summary>
    public RoleEntity Role { get; private set; } = null!;

    /// <summary>
    /// Navigation property for the associated permission.
    /// </summary>
    public PermissionEntity Permission { get; private set; } = null!;
}
