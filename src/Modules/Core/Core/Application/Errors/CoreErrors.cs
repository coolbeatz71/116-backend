using _116.Core.Application.Constants;
using _116.Shared.Application.Exceptions;

namespace _116.Core.Application.Errors;

/// <summary>
/// Core domain error factory providing simple, readable exception creation.
/// Usage: CoreErrors.FileUploadFailed(fileName, reason) or CoreErrors.FileNotFound(fileId)
/// </summary>
public static class CoreErrors
{
    /// <summary>
    /// Throws when file upload fails.
    /// </summary>
    public static ConflictException FileUploadFailed(string fileName, string reason) =>
        new(CoreErrorMessages.File.UploadFailed(fileName, reason));

    /// <summary>
    /// Throws when file type is not supported.
    /// </summary>
    public static BadRequestException UnsupportedFileType(string fileType, string[] allowedTypes) =>
        new(CoreErrorMessages.File.UnsupportedFileType(fileType, allowedTypes));

    /// <summary>
    /// Throws when file size exceeds limit.
    /// </summary>
    public static BadRequestException FileTooLarge(long fileSize, long maxSize) =>
        new(CoreErrorMessages.File.FileTooLarge(fileSize, maxSize));

    /// <summary>
    /// Throws when file is corrupted.
    /// </summary>
    public static BadRequestException CorruptedFile(string fileName) =>
        new(CoreErrorMessages.File.CorruptedFile(fileName));

    /// <summary>
    /// Throws when a file is not found.
    /// </summary>
    public static NotFoundException FileNotFound(int fileId) =>
        new("File", fileId);

    /// <summary>
    /// Throws when a file is not found by name.
    /// </summary>
    public static NotFoundException FileNotFoundByName(string fileName) =>
        new("File", "name", fileName);

    /// <summary>
    /// Throws when configuration is invalid.
    /// </summary>
    public static BadRequestException InvalidConfiguration(string configKey) =>
        new(CoreErrorMessages.System.InvalidConfiguration(configKey));

    /// <summary>
    /// Throws when external service is unavailable.
    /// </summary>
    public static InternalServerException ServiceUnavailable(string serviceName) =>
        new(CoreErrorMessages.System.ServiceUnavailable(serviceName));

    /// <summary>
    /// Throws when database connection fails.
    /// </summary>
    public static InternalServerException DatabaseConnectionFailed() =>
        new(CoreErrorMessages.System.DatabaseConnectionFailed);

    /// <summary>
    /// Throws a generic bad request exception with custom message.
    /// </summary>
    public static BadRequestException BadRequest(string message) =>
        new(message);
}
