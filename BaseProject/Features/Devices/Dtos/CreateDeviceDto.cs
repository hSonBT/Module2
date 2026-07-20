namespace BaseProject.Features.Devices.Dtos;

/// <summary>
/// DTO for creating a device
/// Replaces Laravel Validate\Create rules
/// </summary>
public class CreateDeviceDto
{
    public string Name { get; set; }= string.Empty;
    public string? Description { get; set; }
    public float? Latitude { get; set; }
    public float? Longitude { get; set; }
}