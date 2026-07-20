using BaseProject.Core.Abstractions;
using NetTopologySuite.Geometries;

namespace BaseProject.Features.Devices.Models;

/// <summary>
/// Device entity - represents a GPS tracking device
/// Replaces Laravel Device Model
/// </summary>
public class Device: BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public float? Latitude { get; set; }
    public float? Longitude { get; set; }
    public float? Accuracy { get; set; }
    public bool Active { get; set; }
    public string? Protocol { get; set; }
    public DateTime? LastPositionAt { get; set; }
    
    // Navigation properties (relationships)
    // public ICollection<Position.Models.Position>? Positions { get; set; }
    
}