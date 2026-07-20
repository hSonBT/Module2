namespace BaseProject.Infrastructure.Storage;

/// <summary>
/// Storage provider interface (Strategy Pattern)
/// Replaces Laravel's Storage facade
/// Allows multiple implementations (FileSystem, MinIO, S3, Azure)
/// </summary>
public interface IStorageProvider
{
    /// <summary>
    /// Upload file and return public URL
    /// </summary>
    Task<string> UploadAsync(Stream fileStream, string fileName, string mimeType,
        CancellationToken cancellationToken = default);


    /// <summary>
    /// Download file
    /// </summary>
    Task<Stream> DownloadAsync(string path, CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete file
    /// </summary>
    Task<bool> DeleteAsync(string path, CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if file exists
    /// </summary>
    /// <param name="path"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<bool> ExistsAsync(string path, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get file info (size, modified date, ect)
    /// </summary>
    Task<StorageFileInfo?> GetFileInfoAsync(string path, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get provider name (for logging/debugging)
    /// </summary>
    public string ProviderName { get; }
}

public class StorageFileInfo
{
    public string Path { get; set; } = string.Empty;
    public long Size { get; set; }
    public DateTime ModifiedAt { get; set; }
    public string Url { get; set; } = string.Empty;
}