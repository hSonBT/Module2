using BaseProject.Core.Models;
using BaseProject.Features.Devices.Dtos;

namespace BaseProject.Features.Devices.Commands;

/// <summary>
/// Create device Command
/// Replaces Laravel Device\Action\Create
/// </summary>
public class CreateDeviceCommand: RequestBase<DeviceDto>
{
    public CreateDeviceDto Data { get; set; }
}