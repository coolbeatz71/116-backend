using _116.User.Domain.Entities;
using _116.User.Domain.Enums;

namespace _116.User.Application.Services;

/// <summary>
/// Service interface for JWT token generation and management.
/// Provides methods to create JWT tokens with user authentication and authorization claims.
/// </summary>
public interface IJwtService
{
    /// <summary>
    /// Generates a JWT token containing user identity, roles, permissions, and status information.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="email">The user's email address.</param>
    /// <param name="userName">The user's unique username.</param>
    /// <param name="userRoles">Collection of roles assigned to the user for authorization.</param>
    /// <param name="userPermissions">Collection of permissions granted to the user through roles.</param>
    /// <param name="isVerified">Indicates whether the user's email/account has been verified.</param>
    /// <param name="isActive">Indicates whether the user account is currently active.</param>
    /// <param name="isLoggedIn">Indicates whether the user is currently logged in.</param>
    /// <param name="authProvider">The authentication provider used by the user (Local, Google, Facebook, etc.).</param>
    /// <returns>A JWT token string containing the user's claims and authentication information.</returns>
    /// <remarks>
    /// The generated token includes user identity claims, role-based permissions, account status,
    /// and authentication provider information for comprehensive authorization and session management.
    /// </remarks>
    string GenerateToken(
        Guid userId,
        string email,
        string userName,
        ICollection<UserRoleEntity> userRoles,
        ICollection<RolePermissionEntity> userPermissions,
        bool isVerified,
        bool isActive,
        bool isLoggedIn,
        AuthProvider authProvider
    );
}
