using _116.Shared.Application.Exceptions;
using _116.Shared.Domain;
using _116.User.Domain.Entities;
using _116.User.Domain.ValueObjects;

namespace _116.User.Application.Shared.Repositories;

/// <summary>
/// Repository interface for managing user entities and their related data.
/// Provides methods for user retrieval, validation, and persistence operations.
/// </summary>
public interface IUserRepository : IRepository<UserEntity>
{
    /// <summary>
    /// Retrieves a user with their associated roles by email address.
    /// </summary>
    /// <param name="email">The email address to search for.</param>
    /// <param name="cancellationToken">Token to observe for cancellation requests.</param>
    /// <returns>The user entity with roles loaded.</returns>
    /// <exception cref="NotFoundException">Thrown when no user is found with the specified email.</exception>
    /// <remarks>
    /// This method includes the user's roles in the query for authentication scenarios.
    /// Use this method when you need to validate user credentials and roles.
    /// </remarks>
    Task<UserEntity> GetUserWithRolesOrThrowAsync(Email email, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a user by their unique identifier.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="cancellationToken">Token to observe for cancellation requests.</param>
    /// <returns>The user entity if found; otherwise, null.</returns>
    /// <remarks>
    /// This method performs a simple lookup by primary key without loading related entities.
    /// </remarks>
    Task<UserEntity?> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a user exists with the specified email address.
    /// </summary>
    /// <param name="email">The email address to check for existence.</param>
    /// <param name="cancellationToken">Token to observe for cancellation requests.</param>
    /// <returns>True if a user exists with the email, otherwise, false.</returns>
    /// <remarks>
    /// This method is useful for email uniqueness validation during user registration.
    /// </remarks>
    Task<bool> ExistsByEmailAsync(Email email, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a user exists with the specified username.
    /// </summary>
    /// <param name="userName">The username to check for existence.</param>
    /// <param name="cancellationToken">Token to observe for cancellation requests.</param>
    /// <returns>True if a user exists with the username, otherwise, false.</returns>
    /// <remarks>
    /// This method is useful for username uniqueness validation during user registration.
    /// </remarks>
    Task<bool> ExistsByUserNameAsync(string userName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new user entity to the repository.
    /// </summary>
    /// <param name="user">The user entity to add.</param>
    /// <param name="cancellationToken">Token to observe for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <remarks>
    /// This method only adds the user to the context. Call <see cref="SaveChangesAsync"/> to persist changes.
    /// </remarks>
    Task AddAsync(UserEntity user, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing user entity in the repository.
    /// </summary>
    /// <param name="user">The user entity to update.</param>
    /// <param name="cancellationToken">Token to observe for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <remarks>
    /// This method marks the user as modified and automatically saves changes to the database.
    /// </remarks>
    Task UpdateAsync(UserEntity user, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves an active admin user with roles and permissions by email address.
    /// Validates that the user exists, is active, and has administrative privileges.
    /// </summary>
    /// <param name="email">The email address to search for.</param>
    /// <param name="cancellationToken">Token to observe for cancellation requests.</param>
    /// <returns>The admin user entity with roles and permissions loaded.</returns>
    /// <exception cref="NotFoundException">Thrown when no user is found with the specified email.</exception>
    /// <exception cref="AuthorizationException">Thrown when the user account is inactive (HTTP 403 Forbidden).</exception>
    /// <exception cref="AuthenticationException">Thrown when the user lacks administrative privileges (HTTP 401 Unauthorized).</exception>
    /// <remarks>
    /// This method specifically validates admin privileges and account status with proper HTTP status mapping:
    /// - AuthorizationException: User exists but the account is inactive (403 Forbidden)
    /// - AuthenticationException: User lacks Admin or SuperAdmin role (401 Unauthorized)
    /// Use this for admin authentication scenarios to ensure proper error handling.
    /// </remarks>
    Task<UserEntity> GetActiveAdminUserWithRolesAndPermissionsAsync(Email email, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves an active public user with roles and permissions by credentials (email or username).
    /// Validates that the user exists and is active for public authentication.
    /// </summary>
    /// <param name="credentials">The credentials to search for (email or username).</param>
    /// <param name="cancellationToken">Token to observe for cancellation requests.</param>
    /// <returns>The public user entity with roles and permissions loaded.</returns>
    /// <exception cref="NotFoundException">Thrown when no user is found with the specified credentials.</exception>
    /// <exception cref="AuthorizationException">Thrown when the user account is inactive (HTTP 403 Forbidden).</exception>
    /// <exception cref="AuthorizationException">Thrown when the user account is not verified (HTTP 403 Forbidden).</exception>
    /// <remarks>
    /// This method accepts either email address or username as credentials for public user authentication.
    /// It validates account status with proper HTTP status mapping:
    /// - AuthorizationException: User exists but the account is inactive or not verified (403 Forbidden)
    /// Use this for public user authentication scenarios to ensure proper error handling.
    /// </remarks>
    Task<UserEntity> GetActivePublicUserWithRolesAndPermissionsAsync(string credentials, CancellationToken cancellationToken = default);

    /// <summary>
    /// Persists all pending changes in the repository to the database.
    /// </summary>
    /// <param name="cancellationToken">Token to observe for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <remarks>
    /// This method commits all pending changes (adds, updates, deletes) to the database.
    /// Should be called after performing repository operations that need persistence.
    /// </remarks>
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
