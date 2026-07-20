using BaseProject.Core.Models;
using BaseProject.Features.Devices.Dtos;

namespace BaseProject.Features.Devices.Queries;

/// <summary>
/// Get all devices query
/// Replaces Laravel Device\Active\Index
/// </summary>
public class GetDevicesQuery : RequestBase<IEnumerable<DeviceDto>>
{
    public int Skip { get; set; } = 0;
    public int Take { get; set; } = 10;
    public bool ActiveOnly { get; set; } = false;
}