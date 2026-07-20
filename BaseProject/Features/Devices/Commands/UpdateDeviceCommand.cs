using BaseProject.Core.Models;
using BaseProject.Features.Devices.Dtos;

namespace BaseProject.Features.Devices.Commands;

/// <summary>
/// Create device Command
/// Replaces Laravel Device\Action\Update
/// </summary>
public class UpdateDeviceCommand : RequestBase<DeviceDto>
{
    public int Id { get; set; }
    public UpdateDeviceDto Data { get; set; } = new();
}