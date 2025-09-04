using _116.BuildingBlocks.Constants;
using _116.Shared.Application.Exceptions;
using _116.User.Application.Constants;

namespace _116.User.Application.Errors;

/// <summary>
/// User domain error factory providing simple, readable exception creation.
/// Usage: UserErrors.UserAlreadyExists(email) or UserErrors.UserNotFound(userId)
/// </summary>
public static class UserErrors
{
    /// <summary>
    /// Throws when a user already exists during registration.
    /// </summary>
    public static ConflictException EmailAlreadyExists(string email) =>
        new(UserErrorMessages.Conflict.EmailAlreadyExists(email));

    /// <summary>
    /// Throws when a username is already taken.
    /// </summary>
    public static ConflictException UsernameAlreadyExists(string username) =>
        new(UserErrorMessages.Conflict.UsernameAlreadyExists(username));

    /// <summary>
    /// Throws when a role already exists.
    /// </summary>
    public static ConflictException RoleAlreadyExists(string roleName) =>
        new(UserErrorMessages.Conflict.RoleAlreadyExists(roleName));

    /// <summary>
    /// Throws when a user is not found by ID.
    /// </summary>
    public static NotFoundException UserNotFound(int userId) =>
        new("User", userId);

    /// <summary>
    /// Throws when a user is not found by email.
    /// </summary>
    public static NotFoundException UserNotFoundByEmail(string email) =>
        new("User", "email", email);

    /// <summary>
    /// Throws when a role is not found.
    /// </summary>
    public static NotFoundException RoleNotFound(int roleId) =>
        new("Role", roleId);

    /// <summary>
    /// Throws when a role is not found by name.
    /// </summary>
    public static NotFoundException RoleNotFoundByName(string roleName) =>
        new("Role", "name", roleName);

    /// <summary>
    /// Throws when a permission is not found.
    /// </summary>
    public static NotFoundException PermissionNotFound(int permissionId) =>
        new("Permission", permissionId);

    /// <summary>
    /// Throws when account is inactive.
    /// </summary>
    public static AuthorizationException AccountInactive(string email) =>
        new(UserErrorMessages.Authorization.AccountInactive(email));

    /// <summary>
    /// Throws when account is not verified.
    /// </summary>
    public static AuthorizationException AccountNotVerified(string email) =>
        new(UserErrorMessages.Authorization.AccountNotVerified(email));

    /// <summary>
    /// Throws when account is locked.
    /// </summary>
    public static AuthorizationException AccountLocked(string email) =>
        new(UserErrorMessages.Authorization.AccountLocked(email));

    /// <summary>
    /// Throws when password is invalid.
    /// </summary>
    public static BadRequestException InvalidCredentials() =>
        new(UserErrorMessages.Authentication.InvalidCredentials);

    /// <summary>
    /// Throws when email format is invalid.
    /// </summary>
    public static BadRequestException InvalidEmailFormat(string email) =>
        new(UserErrorMessages.Validation.InvalidEmailFormat(email));

    /// <summary>
    /// Throws when password format is invalid.
    /// </summary>
    public static BadRequestException InvalidPasswordFormat() =>
        new(UserErrorMessages.Validation.InvalidPasswordFormat);

    /// <summary>
    /// Throws when username format is invalid.
    /// </summary>
    public static BadRequestException InvalidUsernameFormat(string username) =>
        new(UserErrorMessages.Validation.InvalidUsernameFormat(username));

    /// <summary>
    /// Throws a generic bad request exception with custom message.
    /// </summary>
    public static BadRequestException BadRequest(string message) =>
        new(message);
}
