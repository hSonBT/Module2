using BaseProject.Core.Models;
using BaseProject.Shared.Dtos;

namespace BaseProject.Shared.Command;

public class UploadAttachmentCommand : RequestBase<AttachmentDto>
{
    public IFormFile File { get; set; } = null;
    public string AttachableType { get; set; } = string.Empty;
    public int AttachableId { get; set; }
    public string? Category { get; set; }
    public bool IsPublic { get; set; } = true;
}