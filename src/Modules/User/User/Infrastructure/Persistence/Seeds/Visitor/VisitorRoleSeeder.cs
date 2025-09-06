using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using _116.Shared.Infrastructure.Seed;
using _116.User.Domain.Entities;
using _116.User.Domain.Enums;
using _116.User.Domain.ValueObjects;

namespace _116.User.Infrastructure.Persistence.Seeds.Visitor;

/// <summary>
/// Seeder responsible for creating the Visitor role with appropriate permissions for public users.
/// </summary>
/// <remarks>
/// Creates the Visitor role as defined in CoreUserRole.Visitor with permissions from VisitorPermissions.
/// All permissions are exactly 8 words describing what the permission allows users to do.
/// </remarks>
public class VisitorRoleSeeder(UserDbContext context, ILogger<VisitorRoleSeeder> logger) : IDataSeeder
{
    /// <inheritdoc />
    public async Task SeedAllAsync()
    {
        try
        {
            logger.LogInformation("Starting Visitor role seeding process...");

            // Check if the visitor role already exists
            bool visitorRoleExists = await context.Roles
                .AnyAsync(r => r.Name == nameof(CoreUserRole.Visitor));

            if (visitorRoleExists)
            {
                logger.LogInformation("Visitor role already exists. Skipping seeding.");
                return;
            }

            // Create the visitor role with permissions
            await ExecuteSeedingAsync();

            logger.LogInformation("Visitor role seeding completed successfully!");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to seed Visitor role data");
            throw;
        }
    }

    /// <summary>
    /// Creates the Visitor role with all the permissions defined in VisitorPermissions.
    /// </summary>
    private async Task ExecuteSeedingAsync()
    {
        // Create Visitor role
        var visitorRole = RoleEntity.Create(
            id: Guid.NewGuid(),
            name: nameof(CoreUserRole.Visitor),
            description: "Standard public/visitor user with content access and interaction permissions"
        );

        // Get all visitor permissions from the typed permissions class
        PermissionEntity[] visitorPermissions = VisitorPermissions.GetAllPermissions();

        // Prepare role-permission associations
        RolePermissionEntity[] rolePermissions = visitorPermissions
            .Select(p => RolePermissionEntity.Create(Guid.NewGuid(), visitorRole.Id, p.Id))
            .ToArray();

        // Add everything in bulk
        await context.Roles.AddAsync(visitorRole);
        await context.Permissions.AddRangeAsync(visitorPermissions);
        await context.RolePermissions.AddRangeAsync(rolePermissions);

        // Persist changes to the database asynchronously
        await context.SaveChangesAsync();

        logger.LogInformation("Created Visitor role with {PermissionCount} permissions", visitorPermissions.Length);
    }
}
