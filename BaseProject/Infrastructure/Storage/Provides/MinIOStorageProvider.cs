using Minio;
using Minio.DataModel.Args;

namespace BaseProject.Infrastructure.Storage.Provides;

/// <summary>
/// MinIO storage provider (S3-compatible)
/// </summary>
public class MinIOStorageProvider : IStorageProvider
{
    private readonly IMinioClient _minioClient;
    private readonly string _bucketName;
    private readonly string _baseUrl;
    private readonly ILogger<MinIOStorageProvider> _logger;

    public string ProviderName => "MinIO";

    public MinIOStorageProvider(
        IConfiguration configuration,
        ILogger<MinIOStorageProvider> logger
    )
    {
        var config = configuration.GetSection("Storage:MinIO");

        var endpoint = config["Endpoint"] ?? "localhost:9000";
        var accessKey = config["AccessKey"] ?? "minioadmin";
        var secretKey = config["SecretKey"] ?? "minioadmin";
        _bucketName = config["BucketName"] ?? "BaseProject";
        _baseUrl = config["BaseUrl"] ?? $"http://{endpoint}/{_bucketName}";

        var useSSL = config.GetValue<bool>("UseSSL", false);
        // Initialize MinIO client
        _minioClient = new MinioClient()
            .WithEndpoint(endpoint)
            .WithCredentials(accessKey, secretKey)
            .WithSSL(useSSL)
            .Build();

        _logger = logger;
    }


    public async Task<string> UploadAsync(Stream fileStream, string fileName, string mimeType,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Ensure bucket exists
            var bucketExistsArgs = new BucketExistsArgs()
                .WithBucket(_bucketName);
            var bucketExists = await _minioClient.BucketExistsAsync(bucketExistsArgs, cancellationToken);

            if (!bucketExists)
            {
                var makeBucketArgs = new MakeBucketArgs()
                    .WithBucket(_bucketName);
                await _minioClient.MakeBucketAsync(makeBucketArgs, cancellationToken);
            }

            // Create unique path
            var objectName = Path.Combine("attachments", DateTime.UtcNow.ToString("yyyy/MM/dd"),
                $"{Guid.NewGuid()}_{fileName}"
            ).Replace("\\", "/");

            // Upload to MinIO
            var putObjectArgs = new PutObjectArgs()
                .WithBucket(_bucketName)
                .WithObject(objectName)
                .WithStreamData(fileStream)
                .WithObjectSize(fileName.Length)
                .WithContentType(mimeType);

            await _minioClient.PutObjectAsync(putObjectArgs, cancellationToken);

            var url = $"{_baseUrl}/{objectName}";

            _logger.LogInformation("File uploaded to MinIO: {FileName} -> {Url}", fileName, url);

            return url;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading file to MinIO: {FileName}", fileName);
            throw;
        }
    }

    public async Task<Stream> DownloadAsync(string path, CancellationToken cancellationToken = default)
    {
        var memoryStream = new MemoryStream();

        var getObjectArgs = new GetObjectArgs()
            .WithBucket(_bucketName)
            .WithObject(path)
            .WithCallbackStream(stream => stream.CopyTo(memoryStream));

        await _minioClient.GetObjectAsync(getObjectArgs, cancellationToken);

        memoryStream.Position = 0;
        return memoryStream;
    }

    public async Task<bool> DeleteAsync(string path, CancellationToken cancellationToken = default)
    {
        try
        {
            var removeObjectArgs = new RemoveObjectArgs()
                .WithBucket(_bucketName)
                .WithObject(path);

            await _minioClient.RemoveObjectAsync(removeObjectArgs, cancellationToken);

            _logger.LogInformation("File deleted from MinIO: {Path}", path);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting file from MinIO: {Path}", path);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(string path, CancellationToken cancellationToken = default)
    {
        try
        {
            var statObjectArgs = new StatObjectArgs()
                .WithBucket(_bucketName)
                .WithObject(path);

            await _minioClient.StatObjectAsync(statObjectArgs, cancellationToken);
            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }

    public async Task<StorageFileInfo?> GetFileInfoAsync(string path, CancellationToken cancellationToken = default)
    {
        try
        {
            var statObjectArgs = new StatObjectArgs()
                .WithBucket(_bucketName)
                .WithObject(path);

            var stat = await _minioClient.StatObjectAsync(statObjectArgs, cancellationToken);
            return new StorageFileInfo()
            {
                Path = path,
                Size = stat.Size,
                ModifiedAt = stat.LastModified,
                Url = $"{_baseUrl}/{path}"
            };
        }
        catch
        {
            return null;
        }
    }
}