namespace BaseProject.Shared.Dtos;

public class AttachmentDto
{
    public int Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string mimeType { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public string? Url { get; set; }
    public string Category { get; set; } = string.Empty;
    public bool IsPublic { get; set; }
    public DateTime CreatedAt { get; set; }

}