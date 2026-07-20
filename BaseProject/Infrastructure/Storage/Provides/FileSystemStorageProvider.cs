namespace BaseProject.Infrastructure.Storage.Provides;

/// <summary>
/// File system storage provider
/// Stores files on local disk
/// </summary>
public class FileSystemStorageProvider : IStorageProvider
{
    private readonly string _basePath;
    private readonly string _baseUrl;
    private readonly ILogger<FileSystemStorageProvider> _logger;

    public string ProviderName => "FileSystem";

    public FileSystemStorageProvider(
        IConfiguration configuration,
        ILogger<FileSystemStorageProvider> logger)
    {
        var config = configuration.GetSection("Storage:FileSystem");
        _basePath = config["BasePath"] ?? Path.Combine(Directory.GetCurrentDirectory(), "storage");
        _logger = logger;

        // create base dictionary if not exists
        if (!Directory.Exists(_basePath))
        {
            Directory.CreateDirectory(_basePath);
        }
    }

    public async Task<string> UploadAsync(Stream fileStream, string fileName, string mimeType,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Create unique path
            var relativePath = Path.Combine(
                "attachments",
                DateTime.UtcNow.ToString("yyyy/MM/dd"),
                $"{Guid.NewGuid()}_{fileName}"
            );

            var fullPath = Path.Combine(_basePath, relativePath);
            var directory = Path.GetDirectoryName(fullPath);

            // Create directory if not exists
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory!);
            }

            // Save file
            await using var fileWriteStream = new FileStream(
                fullPath,
                FileMode.Create,
                FileAccess.Write
            );

            await fileStream.CopyToAsync(fileWriteStream, cancellationToken);

            var url = $"{_baseUrl}/{relativePath.Replace("\\", "/")}";
            _logger.LogInformation("File uploaded: {FileName} -> {Url}", fileName, url);
            return url;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading file: {FileName}", fileName);
            throw;
        }
    }

    public async Task<Stream> DownloadAsync(string path, CancellationToken cancellationToken = default)
    {
        var fullPath = Path.Combine(_basePath, path);

        if (!File.Exists(fullPath))
            throw new FileNotFoundException($"File not found: {path}");

        return new FileStream(fullPath,
            FileMode.Open,
            FileAccess.Read);
    }

    public async Task<bool> DeleteAsync(string path, CancellationToken cancellationToken = default)
    {
        try
        {
            var fullPath = Path.Combine(_basePath, path);
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
                _logger.LogInformation("File deleted: {Path}", path);
                return true;
            }

            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting file: {Path}", path);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(string path, CancellationToken cancellationToken = default)
    {
        var fullPath = Path.Combine(_basePath, path);
        return File.Exists(fullPath);
    }

    public async Task<StorageFileInfo?> GetFileInfoAsync(string path, CancellationToken cancellationToken = default)
    {
        var fullPath = Path.Combine(_basePath, path);
        if (!File.Exists(fullPath))
            return null;

        var fileInfo = new FileInfo(fullPath);
        return new StorageFileInfo()
        {
            Path = path,
            Size = fileInfo.Length,
            ModifiedAt = fileInfo.LastWriteTimeUtc,
            Url = $"{_baseUrl}/{path.Replace("\\", "/")}"
        };
    }
}