namespace _116.Core.Application.Shared.Errors.Messages;

/// <summary>
/// Provides internal server error messages for the <c>Core</c> domain.
/// These messages describe system-level failures and service unavailability.
/// </summary>
public static class InternalServerErrorMessage
{
    /// <summary>
    /// Gets the error message for service unavailable.
    /// </summary>
    /// <param name="serviceName">The name of the unavailable service.</param>
    /// <returns>A formatted error message indicating the service is unavailable.</returns>
    public static string ServiceUnavailable(string serviceName)
    {
        return $"Service '{serviceName}' is currently unavailable";
    }

    /// <summary>
    /// Error message for database connection failure.
    /// </summary>
    /// <returns>A formatted error message indicating database connection failed.</returns>
    public static string DatabaseConnectionFailed()
    {
        return "Unable to connect to the database";
    }
}
