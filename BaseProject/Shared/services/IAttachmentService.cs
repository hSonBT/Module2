using BaseProject.Shared.Dtos;

namespace BaseProject.Shared.services;

public interface IAttachmentService
{
    Task<AttachmentDto> UploadAsync(
        UploadAttachmentDto uploadDto,
        int enterpriseId,
        int? userId,
        CancellationToken cancellationToken = default
    );

    Task<IEnumerable<AttachmentDto>> GetByAttachableAsync(
        string attachableType,
        int attachableId,
        CancellationToken cancellationToken = default
    );

    Task<AttachmentDto?> GetByIdAsync(int attachmentId, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(int attachmentId, CancellationToken cancellationToken = default);

    Task<Stream> DownloadAsync(int attachmentId, CancellationToken cancellationToken = default);
}