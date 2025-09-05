using _116.Core.Domain.DTOs;
using _116.User.Domain.DTOs;
using _116.User.Domain.Entities;
using Mapster;

namespace _116.User.Application.Shared.Mappers;

/// <summary>
/// Mapster configuration for User entity mappings.
/// Leverages automatic mapping with minimal manual configuration.
/// </summary>
public static class UserMapper
{
    /// <summary>
    /// Configures Mapster mappings once at application startup.
    /// Uses automatic property mapping with selective overrides.
    /// </summary>
    public static void Configure()
    {
        // Configure UserEntity to UserResponseDto - leverage automatic mapping for matching properties
        TypeAdapterConfig<UserEntity, UserResponseDto>
            .NewConfig()
            .Map(dest => dest.Roles, _ => new List<RoleDto>()) // Default empty collection
            .Map(dest => dest.Permissions, _ => new List<PermissionDto>()) // Default empty collection
            .Map(dest => dest.Avatar, _ => (FileDto?)null) // Null until core file implemented
            .Map(dest => dest.AuthProvider, src => src.AuthProvider.ToString()) // Convert enum to string
            .Compile(); // Compile for performance
    }

    /// <summary>
    /// High-performance extension method to map UserEntity to UserResponseDto with roles and permissions.
    /// Leverage Mapster's compiled mappings with selective property override.
    /// </summary>
    /// <param name="user">User entity to map</param>
    /// <param name="roles">User roles collection</param>
    /// <param name="permissions">User permissions collection</param>
    /// <returns>Fully populated UserResponseDto</returns>
    public static UserResponseDto ToUserResponseDto(
        this UserEntity user,
        IReadOnlyCollection<RoleDto> roles,
        IReadOnlyCollection<PermissionDto> permissions
    )
    {
        // Use base mapping then override specific collections - avoids repetitive property mapping
        var dto = user.Adapt<UserResponseDto>();

        // Only override the collections that can't be auto-mapped
        return dto with
        {
            Roles = roles,
            Permissions = permissions
        };
    }
}
