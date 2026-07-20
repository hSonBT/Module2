using AutoMapper;
using BaseProject.Infrastructure.Data;
using BaseProject.Infrastructure.Storage;
using BaseProject.Shared.Dtos;
using BaseProject.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace BaseProject.Shared.services;

public class AttachmentService : IAttachmentService
{
    private readonly MyAppDbContext _dbContext;
    private readonly IStorageProvider _storageProvider;
    private readonly IMapper _mapper;
    private readonly ILogger<AttachmentService> _logger;

    public AttachmentService(
        MyAppDbContext dbContext,
        IStorageProvider storageProvider,
        IMapper mapper,
        ILogger<AttachmentService> logger)
    {
        _dbContext = dbContext;
        _storageProvider = storageProvider;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<AttachmentDto> UploadAsync(UploadAttachmentDto uploadDto, int enterpriseId, int? userId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Upload file to storage provider
            await using var stream = uploadDto.File.OpenReadStream();
            var url = await _storageProvider.UploadAsync(
                stream,
                uploadDto.File.FileName,
                uploadDto.File.ContentType,
                cancellationToken
            );

            // Create attachment record
            var attachment = new Attachment()
            {
                FileName = uploadDto.File.FileName,
                MimeType = uploadDto.File.ContentType,
                FileSize = uploadDto.File.Length,
                StoragePath = ExtractParamFromUrl(url),
                Url = url,
                AttachableType = uploadDto.AttachableType,
                AttachableId = uploadDto.AttachableId,
                Category = uploadDto.Category,
                IsPublic = uploadDto.IsPublic,
                UserId = userId,
                EnterpriseId = enterpriseId,
                CreatedAt = DateTime.UtcNow
            };

            await _dbContext.Attachments.AddAsync(attachment, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Attachment uploaded: {FileName} for {AttachableType}:{AttachableId}",
                uploadDto.File.FileName,
                uploadDto.AttachableType,
                uploadDto.AttachableId);
            return _mapper.Map<AttachmentDto>(attachment);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading attachment");
            throw;
        }
    }

    public async Task<IEnumerable<AttachmentDto>> GetByAttachableAsync(string attachableType, int attachableId,
        CancellationToken cancellationToken = default)
    {
        var attachments = await _dbContext.Attachments
            .Where(a => a.AttachableType == attachableType && a.AttachableId == attachableId)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync(cancellationToken);
        return _mapper.Map<IEnumerable<AttachmentDto>>(attachments);
    }

    public async Task<AttachmentDto?> GetByIdAsync(int attachmentId, CancellationToken cancellationToken = default)
    {
        var attachment =
            await _dbContext.Attachments.FindAsync(
                new object[] { attachmentId },
                cancellationToken: cancellationToken
            );
        return attachment != null ? _mapper.Map<AttachmentDto>(attachment) : null;
    }

    public async Task<bool> DeleteAsync(int attachmentId, CancellationToken cancellationToken = default)
    {
        var attachment = await _dbContext.Attachments.FindAsync(new object[] { attachmentId }, cancellationToken);

        if (attachment == null)
            return false;

        try
        {
            // Detele from storage
            await _storageProvider.DeleteAsync(attachment.StoragePath, cancellationToken);

            // Delete record
            _dbContext.Attachments.Remove(attachment);
            await _dbContext.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Attachment deleted: {AttachmentId}", attachmentId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting attachment: {AttachmentId}", attachmentId);
            throw;
        }
    }

    public async Task<Stream> DownloadAsync(int attachmentId, CancellationToken cancellationToken = default)
    {
        var attachment = await _dbContext.Attachments.FindAsync(
            new object[] { attachmentId }, cancellationToken);
        
        if(attachment == null)
            throw new FileNotFoundException($"Attachment: {attachmentId} not found");
        
        return await _storageProvider.DownloadAsync(attachment.StoragePath, cancellationToken);
    }

    private string ExtractParamFromUrl(string url)
    {
        // Extract path from URL (remove base URL part)
        var uri = new Uri(url);
        return uri.PathAndQuery.TrimStart('/');
    }
}