using BaseProject.Core.Exceptions;
using BaseProject.Shared.Command;
using BaseProject.Shared.Dtos;
using BaseProject.Shared.services;
using MediatR;

namespace BaseProject.Shared.Handler;

public class UploadAttachmentCommandHandler : IRequestHandler<UploadAttachmentCommand, AttachmentDto>
{
    private readonly IAttachmentService _attachmentService;
    private readonly ILogger<UploadAttachmentCommandHandler> _logger;
    private readonly long _maxFileSize = 50 * 1024 * 1024; // 50MB

    public UploadAttachmentCommandHandler(IAttachmentService attachmentService,
        ILogger<UploadAttachmentCommandHandler> logger)
    {
        _attachmentService = attachmentService;
        _logger = logger;
    }


    public async Task<AttachmentDto> Handle(UploadAttachmentCommand request, CancellationToken cancellationToken)
    {
        // Validation file size
        if (request.File.Length > _maxFileSize)
            throw new ValidationException("File size exceeds 50MB limit");

        // Validation file type
        var allowedMimeTypes = new[] { "image/jpeg", "image/png", "application/pdf", "video/mp4" };
        if (!allowedMimeTypes.Contains(request.File.ContentType))
            throw new ValidationException("File type no allowed");

        var uploadDto = new UploadAttachmentDto()
        {
            File = request.File,
            AttachableType = request.AttachableType,
            AttachableId = request.AttachableId,
            Category = request.Category,
            IsPublic = request.IsPublic,
        };

        return await _attachmentService.UploadAsync(
            uploadDto,
            request.EnterpriseId ?? 1,
            request.UserId,
            cancellationToken
            );
    }
}