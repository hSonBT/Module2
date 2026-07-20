using BaseProject.Core.Models;
using BaseProject.Features.Devices.Dtos;

namespace BaseProject.Features.Devices.Commands;

/// <summary>
/// Create device Command
/// Replaces Laravel Device\Action\Delete
/// </summary>
public class DeleteDeviceCommand : RequestBase<bool>
{
    public int Id { get; set; }
}