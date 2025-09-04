namespace _116.Core.Application.Constants;

/// <summary>
/// Error messages for the Core domain.
/// </summary>
public static class CoreErrorMessages
{
    /// <summary>
    /// File-related error messages.
    /// </summary>
    public static class File
    {
        /// <summary>
        /// Gets error message for upload failure.
        /// </summary>
        public static string UploadFailed(string fileName, string reason) =>
            $"Failed to upload file '{fileName}': {reason}";

        /// <summary>
        /// Gets error message for unsupported file type.
        /// </summary>
        public static string UnsupportedFileType(string fileType, string[] allowedTypes) =>
            $"File type '{fileType}' is not supported. Allowed types: {string.Join(", ", allowedTypes)}";

        /// <summary>
        /// Gets error message for file too large.
        /// </summary>
        public static string FileTooLarge(long fileSize, long maxSize) =>
            $"File size {fileSize} bytes exceeds maximum allowed size of {maxSize} bytes";

        /// <summary>
        /// Gets error message for corrupted file.
        /// </summary>
        public static string CorruptedFile(string fileName) =>
            $"File '{fileName}' appears to be corrupted or invalid";
    }

    /// <summary>
    /// System-related error messages.
    /// </summary>
    public static class System
    {
        /// <summary>
        /// Gets error message for invalid configuration.
        /// </summary>
        public static string InvalidConfiguration(string configKey) =>
            $"Configuration '{configKey}' is missing or invalid";

        /// <summary>
        /// Gets error message for service unavailable.
        /// </summary>
        public static string ServiceUnavailable(string serviceName) =>
            $"Service '{serviceName}' is currently unavailable";

        /// <summary>
        /// Error message for database connection failure.
        /// </summary>
        public const string DatabaseConnectionFailed = "Unable to connect to the database";
    }
}
