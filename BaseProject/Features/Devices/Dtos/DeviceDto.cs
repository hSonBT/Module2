namespace BaseProject.Features.Devices.Dtos;

public class DeviceDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public float? Latitude { get; set; }
    public float? Longitude { get; set; }
    public float? Accuracy { get; set; }
    public bool Active { get; set; }
    public string? Protocol { get; set; }
    public DateTime? LastPositionAt { get; set; }
    public int? UserId { get; set; }
    public int EnterpriseId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}