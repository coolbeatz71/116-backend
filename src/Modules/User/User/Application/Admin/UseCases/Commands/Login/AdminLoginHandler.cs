using _116.Shared.Application.Exceptions;
using _116.Shared.Contracts.Application.CQRS;
using _116.User.Application.Services;
using _116.User.Application.Shared.Repositories;
using _116.User.Application.Shared.Services;
using _116.User.Domain.DTOs;
using _116.User.Domain.Entities;
using _116.User.Domain.Enums;
using _116.User.Domain.Results;
using _116.User.Domain.ValueObjects;
using Mapster;

namespace _116.User.Application.Admin.UseCases.Commands.Login;

/// <summary>
/// Handles the <see cref="AdminLoginCommand"/> to authenticate admin users.
/// </summary>
/// <param name="userRepository">Repository for user data access operations.</param>
/// <param name="passwordService">Service for verifying hashed passwords.</param>
/// <param name="jwtService">Service for generating JWT tokens with admin claims.</param>
public class AdminLoginHandler(
    IUserRepository userRepository,
    IPasswordService passwordService,
    IJwtService jwtService
) : ICommandHandler<AdminLoginCommand, AdminLoginResult>
{
    /// <summary>
    /// Handles the admin login command by authenticating the user and validating admin privileges.
    /// </summary>
    /// <param name="command">The admin login command containing credentials.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>An <see cref="AdminLoginResult"/> containing admin authentication information.</returns>
    /// <exception cref="BadRequestException">Thrown when credentials are invalid or user lacks admin privileges.</exception>
    /// <exception cref="AuthenticationException">Thrown for users without admin privileges.</exception>
    public async Task<AdminLoginResult> Handle(AdminLoginCommand command, CancellationToken cancellationToken)
    {
        // Normalize email using value object
        var email = new Email(command.Email);

        // Get user with roles and permissions for complete authentication
        UserEntity user = await userRepository.GetUserWithRolesOrThrowAsync(
            email,
            cancellationToken
        );

        // Verify password
        if (!passwordService.Verify(command.Password, user.PasswordHash))
        {
            throw new BadRequestException("Invalid email or password.");
        }

        // Validate admin privileges - user must have Admin or SuperAdmin role
        bool hasAdminRole = user.UserRoles
            .Any(ur => ur.Role.Name is nameof(CoreUserRole.Admin) or nameof(CoreUserRole.SuperAdmin));

        if (!hasAdminRole)
        {
            throw new AuthenticationException("Access denied. Administrative privileges required.");
        }

        // Extract user permissions from roles
        List<RolePermissionEntity> userPermissions = user.UserRoles
            .SelectMany(ur => ur.Role.RolePermissions)
            .ToList();

        // Generate JWT token with admin claims
        JwtGenerationResult token = jwtService.GenerateToken(
            userId: user.Id,
            email: email.Value,
            userName: user.UserName,
            userRoles: user.UserRoles,
            userPermissions: userPermissions,
            isVerified: user.IsVerified,
            isActive: user.IsActive,
            isLoggedIn: user.IsLoggedIn,
            authProvider: user.AuthProvider
        );

        // Record successful login
        user.RecordLogin();
        await userRepository.SaveChangesAsync(cancellationToken);

        var userDto = user.Adapt<UserResponseDto>();
        var authResult = new AuthenticationResult(userDto, token.Token, token.ExpiresAt);

        return new AdminLoginResult(authResult);
    }
}
