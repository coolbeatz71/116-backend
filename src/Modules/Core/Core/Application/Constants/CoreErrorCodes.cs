namespace _116.Core.Application.Constants;

/// <summary>
/// Error codes specific to the Core domain.
/// </summary>
public static class CoreErrorCodes
{
    /// <summary>
    /// File-related error codes.
    /// </summary>
    public static class File
    {
        /// <summary>
        /// File upload failed.
        /// </summary>
        public const string UploadFailed = "FILE_UPLOAD_FAILED";

        /// <summary>
        /// File type not supported.
        /// </summary>
        public const string UnsupportedFileType = "UNSUPPORTED_FILE_TYPE";

        /// <summary>
        /// File size exceeds maximum allowed.
        /// </summary>
        public const string FileTooLarge = "FILE_TOO_LARGE";

        /// <summary>
        /// File is corrupted or invalid.
        /// </summary>
        public const string CorruptedFile = "CORRUPTED_FILE";
    }

    /// <summary>
    /// System-related error codes.
    /// </summary>
    public static class System
    {
        /// <summary>
        /// Configuration is missing or invalid.
        /// </summary>
        public const string InvalidConfiguration = "INVALID_CONFIGURATION";

        /// <summary>
        /// External service is unavailable.
        /// </summary>
        public const string ServiceUnavailable = "SERVICE_UNAVAILABLE";

        /// <summary>
        /// Database connection failed.
        /// </summary>
        public const string DatabaseConnectionFailed = "DATABASE_CONNECTION_FAILED";
    }
}
