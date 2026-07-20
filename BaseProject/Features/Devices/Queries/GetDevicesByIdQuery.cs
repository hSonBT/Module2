using BaseProject.Core.Models;
using BaseProject.Features.Devices.Dtos;

namespace BaseProject.Features.Devices.Queries;

/// <summary>
/// Get device by id query
/// Replaces Laravel Device\Active\...
/// </summary>
public class GetDevicesByIdQuery : RequestBase<DeviceDto>
{
    public int Id { get; set; }
}