using _116.Shared.Application.Metadata;

namespace _116.User.Application.Admin.UseCases.Commands.Login;

/// <summary>
/// Contains metadata information for the admin login route.
/// </summary>
public static class AdminLoginMetaField
{
    public static readonly RouteMetadata AdminLogin = new(
        name: "AdminLogin",
        summary: "Authenticate admin and return JWT token with admin claims",
        description: """
             Authenticates an admin user using email and password credentials.

             This endpoint performs enhanced authentication by:
             - Validating email and password
             - Verifying the account is active and verified
             - Checking for admin role privileges (Admin or SuperAdmin)
             - Generating JWT token with appropriate admin claims
             - Recording the login activity

             **Authentication Requirements:**
             - Valid email and password combination
             - Account must be active and verified
             - User must have Admin or SuperAdmin role assigned

             **Security Features:**
             - Password verification using secure hashing (bcrypt)
             - Role-based access validation
             - Login activity tracking
             - Enhanced JWT claims for admin operations

             **Response:**
             - Returns 200 OK with user info and JWT token on success
             - Returns 400 Bad Request for invalid credentials
             - Returns 401 Unauthorized for users without admin privileges

             The returned JWT token includes admin-specific claims for accessing administrative endpoints.
         """
    );
}

