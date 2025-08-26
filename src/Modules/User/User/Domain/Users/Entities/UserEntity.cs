using System.ComponentModel.DataAnnotations;
using _116.Core.Domain;
using _116.User.Domain.Users.Enums;

namespace _116.User.Domain.Users.Entities;

/// <summary>
/// Represents a user in the authentication system, containing identity, credential,
/// and profile-related information.
/// </summary>
/// <remarks>
/// This entity serves as an aggregate root with a unique identifier, managing
/// authentication and profile data such as email, password hash, roles, and login state.
/// </remarks>
public class UserEntity : Aggregate<Guid>
{
    /// <summary>
    /// Email address of the user.
    /// May be <c>null</c> for external authentication providers (e.g., Facebook).
    /// </summary>
    public string? Email { get; private set; }

    /// <summary>
    /// Unique username chosen by the user (max 20 characters).
    /// </summary>
    [MaxLength(20)]
    public string UserName { get; private set; } = null!;

    /// <summary>
    /// Hashed password used for authentication.
    /// </summary>
    public string PasswordHash { get; private set; } = null!;

    /// <summary>
    /// Identifier of the user's avatar file, if uploaded.
    /// </summary>
    public Guid? AvatarFileId { get; private set; }

    /// <summary>
    /// Navigation property:
    /// Collection of roles assigned to the user.
    /// </summary>
    public ICollection<UserRoleEntity> UserRoles { get; private set; } = new List<UserRoleEntity>();

    /// <summary>
    /// Authentication provider used by the user (e.g., Local, Google, Facebook).
    /// </summary>
    public AuthProvider AuthProvider { get; private set; }

    /// <summary>
    /// Indicates whether the user's email/account has been verified.
    /// </summary>
    public bool IsVerified { get; private set; } = false;

    /// <summary>
    /// Indicates whether the user account is currently active.
    /// </summary>
    public bool IsActive { get; private set; }

    /// <summary>
    /// Indicates whether the user is currently logged in.
    /// </summary>
    public bool IsLoggedIn { get; private set; }

    /// <summary>
    /// Date and time of the user's last login, in UTC.
    /// </summary>
    public DateTime? LastLoginAt { get; private set; }

    /// <summary>
    /// Full country name associated with the user.
    /// </summary>
    public string? CountryName { get; private set; }

    /// <summary>
    /// URL link to the flag of the user's country.
    /// </summary>
    public string? CountryFlagUrl { get; private set; }

    /// <summary>
    /// ISO country code (e.g., "US", "RW").
    /// </summary>
    public string? CountryIsoCode { get; private set; }

    /// <summary>
    /// Country dialing code (e.g., "+1", "+250").
    /// </summary>
    public string? CountryDialCode { get; private set; }

    /// <summary>
    /// Partial (masked) phone number for privacy display.
    /// </summary>
    public string? PartialPhoneNumber { get; private set; }

    /// <summary>
    /// Full phone number including country code.
    /// </summary>
    public string? FullPhoneNumber { get; private set; }
}
