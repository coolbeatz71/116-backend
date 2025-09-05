namespace _116.Core.Application.Shared.Errors.Messages;

/// <summary>
/// Provides conflict-related error messages for the <c>Core</c> domain.
/// These messages describe situations where operations cannot proceed due to conflicts.
/// </summary>
public static class ConflictErrorMessage
{
    /// <summary>
    /// Gets error message for file upload failure.
    /// </summary>
    /// <param name="fileName">The name of the file that failed to upload.</param>
    /// <param name="reason">The reason for the upload failure.</param>
    /// <returns>A formatted error message indicating the upload failed.</returns>
    public static string FileUploadFailed(string fileName, string reason)
    {
        return $"Failed to upload file '{fileName}': {reason}";
    }
}
