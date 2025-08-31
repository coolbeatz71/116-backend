namespace _116.User.Domain.Enums;

/// <summary>
/// Defines the core user roles within the system hierarchy.
/// </summary>
/// <remarks>
/// This enumeration contains only the foundational administrative roles that form the base
/// of the user management system. These are immutable system roles that cannot be created
/// or modified at runtime.
///
/// <para>
/// <strong>Role Hierarchy and Permissions:</strong>
/// </para>
/// <list type="bullet">
/// <item><see cref="SuperAdmin"/> - Has complete system control and can manage all aspects including user roles</item>
/// <item><see cref="Admin"/> - Has administrative privileges but cannot modify core system roles</item>
/// </list>
///
/// <para>
/// <strong>Additional Role Assignment:</strong>
/// </para>
/// Users with <see cref="Admin"/> or <see cref="SuperAdmin"/> roles can be assigned additional
/// domain-specific roles (e.g., Moderator, AdsManager, ContentManager) through the permission
/// system. These additional roles are created and managed by <see cref="SuperAdmin"/> users
/// and stored separately from these core system roles.
///
/// <para>
/// <strong>Usage Example:</strong>
/// </para>
/// A user might have <see cref="Admin"/> as their base role, plus additional permissions for
/// "Moderator" and "AdsManager" functions, allowing them to moderate content and manage
/// advertisements while maintaining their core administrative privileges.
/// </remarks>
public enum CoreUserRole
{
    /// <summary>
    /// Administrative role with elevated system privileges.
    /// </summary>
    /// <remarks>
    /// Users with this role have administrative access to most system functions but cannot
    /// modify core system roles or perform super-admin-only operations. They can be assigned
    /// additional domain-specific roles and permissions as needed.
    /// </remarks>
    Admin,

    /// <summary>
    /// Highest privilege role with complete system control.
    /// </summary>
    /// <remarks>
    /// Users with this role have unrestricted access to all system functions, including
    /// the ability to create, modify, and assign additional roles and permissions to other users.
    /// This role should be assigned sparingly and only to trusted system administrators.
    /// </remarks>
    SuperAdmin
}
