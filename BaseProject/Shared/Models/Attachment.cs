using BaseProject.Core.Abstractions;

namespace BaseProject.Shared.Models;

/// <summary>
/// Attachment entity - polymorphic file storage
/// Replaces Laravel attachment pattern
/// Can be used for Device, Trip, Position, or any entity
/// </summary>
public class Attachment : BaseEntity
{
    /// <summary>
    /// Original file name
    /// </summary>
    public string FileName { get; set; } = string.Empty;

    /// <summary>
    /// MIME type (image/png, application/pdf, etc)
    /// </summary>
    public string MimeType { get; set; } = string.Empty;

    /// <summary>
    /// File size in bytes
    /// </summary>
    public long FileSize { get; set; }

    /// <summary>
    /// Path in storage provider (e.g., "attachments/devices/123/file.jpg")
    /// </summary>
    public string StoragePath { get; set; } = string.Empty;

    /// <summary>
    /// Public URL to access file
    /// </summary>
    public string? Url { get; set; }

    /// <summary>
    /// Polymorphic type (Device, Trip, Position, etc)
    /// </summary>
    public string AttachableType { get; set; } = string.Empty;

    /// <summary>
    /// Polymorphic ID (device ID, position ID, etc)
    /// </summary>
    public int AttachableId { get; set; }

    /// <summary>
    /// File category (image, document, video, etc)
    /// </summary>
    public string? Category { get; set; }

    /// <summary>
    /// Additional metadata as JSON
    /// </summary>
    public string? Metadata { get; set; }

    /// <summary>
    /// Whether file is public or private
    /// </summary>
    public bool IsPublic { get; set; } = true;
}