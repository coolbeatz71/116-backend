using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using _116.BuildingBlocks.Constants;
using _116.Core.Application.Configurations;
using _116.User.Domain.Entities;
using _116.User.Domain.Enums;
using _116.User.Domain.Results;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace _116.User.Application.Services;

/// <summary>
/// Service responsible for generating JWT tokens with user claims, roles, and permissions.
/// </summary>
/// <param name="configuration">Application configuration for JWT settings</param>
public class JwtService(IConfiguration configuration) : IJwtService
{
    /// <inheritdoc />
    public JwtGenerationResult GenerateToken(
        Guid userId,
        string email,
        string userName,
        ICollection<UserRoleEntity> userRoles,
        ICollection<RolePermissionEntity> userPermissions,
        bool isVerified,
        bool isActive,
        bool isLoggedIn,
        AuthProvider authProvider
    )
    {
        var (secret, issuer, audience, expiration) = AppEnvironment.Jwt();

        if (string.IsNullOrWhiteSpace(secret))
        {
            throw new InvalidOperationException("JWT_SECRET env variable is missing or empty.");
        }

        DateTimeOffset now = DateTimeOffset.UtcNow;
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, $"{userId}"),
            new Claim(ClaimTypes.Name, userName),
            new Claim(ClaimTypes.Email, email),
            new Claim(JwtRegisteredClaimNames.Sub, $"{userId}"),
            new Claim(JwtRegisteredClaimNames.Email, email),
            new Claim(JwtRegisteredClaimNames.Jti, $"{Guid.NewGuid()}"),
            new Claim(JwtRegisteredClaimNames.Iat, $"{now.ToUnixTimeSeconds()}", ClaimValueTypes.Integer64),
            new Claim(JwtClaimsConstants.AuthProvider, $"{authProvider}")
        };

        claims.AddRange(BuildRoleClaims(userRoles));
        claims.AddRange(BuildPermissionClaims(userPermissions));
        claims.AddRange(BuildStatusClaims(isVerified, isActive, isLoggedIn));

        int expirationHours = int.TryParse(expiration, out int parsed)
            ? parsed
            : CoreConstants.JwtDefaultExpiration;

        DateTime expiresAt = now.AddHours(expirationHours).UtcDateTime;

        var descriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = expiresAt,
            SigningCredentials = credentials,
            Issuer = issuer,
            Audience = audience
        };

        var handler = new JwtSecurityTokenHandler();
        string? token = handler.WriteToken(handler.CreateToken(descriptor));

        return new JwtGenerationResult(token, expiresAt);
    }

    /// <summary>
    /// Builds account status claims for the JWT token.
    /// </summary>
    /// <param name="isVerified">Whether the user's account is verified</param>
    /// <param name="isActive">Whether the user's account is active</param>
    /// <param name="isLoggedIn">Whether the user is currently logged in</param>
    /// <returns>Collection of status claims</returns>
    private static IEnumerable<Claim> BuildStatusClaims(bool isVerified, bool isActive, bool isLoggedIn)
    {
        return new Dictionary<string, bool>
        {
            [JwtClaimsConstants.IsVerified] = isVerified,
            [JwtClaimsConstants.IsActive] = isActive,
            [JwtClaimsConstants.IsLoggedIn] = isLoggedIn
        }.Select(kvp => new Claim(kvp.Key, kvp.Value ? "true" : "false", ClaimValueTypes.Boolean));
    }

    /// <summary>
    /// Builds role claims from the user's assigned roles.
    /// </summary>
    /// <param name="userRoles">Collection of user role entities</param>
    /// <returns>Collection of role claims</returns>
    private static IEnumerable<Claim> BuildRoleClaims(ICollection<UserRoleEntity> userRoles)
    {
        return userRoles.Select(r => new Claim(ClaimTypes.Role, r.Role.Name));
    }

    /// <summary>
    /// Builds permission claims from the user's assigned permissions.
    /// </summary>
    /// <param name="permissions">Collection of role permission entities</param>
    /// <returns>Collection of permission claims in the format "resource:action"</returns>
    private static IEnumerable<Claim> BuildPermissionClaims(ICollection<RolePermissionEntity> permissions)
    {
        return permissions
            .Select(p => $"{p.Permission.Resource}:{p.Permission.Action}")
            .Select(v => new Claim(JwtClaimsConstants.Permission, v));
    }
}
