namespace _116.Core.Application;

/// <summary>
/// Defines standardized route prefixes for different API endpoint groups.
/// </summary>
public class RouteGroup
{
    /// <summary>
    /// Gets the base API version string.
    /// </summary>
    private static string Version => "api/v1";

    /// <summary>
    /// Gets the route prefix for authentication endpoints.
    /// </summary>
    public static string Auth => $"{Version}/auth";

    /// <summary>
    /// Gets the route prefix for user management endpoints.
    /// </summary>
    public static string User => $"{Version}/users";
}
