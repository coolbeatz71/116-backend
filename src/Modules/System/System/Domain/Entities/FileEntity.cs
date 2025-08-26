using _116.Core.Domain;

namespace _116.System.Domain.Entities;

/// <summary>
/// Represents a file uploaded to the system, containing metadata and storage information
/// for managing files across the application.
/// </summary>
/// <remarks>
/// This entity serves as an aggregate root for file management, storing both the original
/// file information and the system-generated metadata for storage and retrieval.
/// </remarks>
public class FileEntity : Aggregate<Guid>
{
    /// <summary>
    /// System-generated unique filename used for storage.
    /// </summary>
    public string FileName { get; private set; } = null!;

    /// <summary>
    /// Original filename as uploaded by the user.
    /// </summary>
    public string OriginalFileName { get; private set; } = null!;

    /// <summary>
    /// MIME type of the file (e.g., "image/jpeg", "application/pdf").
    /// </summary>
    public string MimeType { get; private set; } = null!;

    /// <summary>
    /// URL or path where the file is stored in the storage system.
    /// </summary>
    public string StorageUrl { get; private set; } = null!;
}
