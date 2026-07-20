using BaseProject.Core.Abstractions;
using MediatR;

namespace BaseProject.Features.Devices.Controllers;

public class DeviceController: BaseApiController
{
    public DeviceController(IMediator mediator, ILogger<DevicesController> logger) : base(mediator, logger)
    {
    }
}