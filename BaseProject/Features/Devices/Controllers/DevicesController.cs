using BaseProject.Core.Abstractions;
using BaseProject.Features.Devices.Commands;
using BaseProject.Features.Devices.Dtos;
using BaseProject.Features.Devices.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BaseProject.Features.Devices.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DevicesController : BaseApiController
{
    public DevicesController(IMediator mediator, ILogger<DevicesController> logger) : base(mediator, logger)
    {
    }


    /// <summary>
    /// Get all devices
    /// GET /api/devices
    ///</summary>
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int skip = 0,
        [FromQuery] int take = 10,
        [FromQuery] bool activeOnly = false,
        CancellationToken cancellationToken = default
    )
    {
        var query = new GetDevicesQuery()
        {
            Skip = skip,
            Take = take,
            ActiveOnly = activeOnly,
            EnterpriseId = 1 // TODO: Get from claims
        };

        return await ExecuteQuery<GetDevicesQuery, IEnumerable<DeviceDto>>(query);
    }


    /// <summary>
    /// Get Device by ID
    /// GET /api/devices/1
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id, CancellationToken cancellationToken = default)
    {
        var query = new GetDevicesByIdQuery() { Id = id };
        return await ExecuteQuery<GetDevicesByIdQuery, DeviceDto>(query);
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateDeviceDto request,
        CancellationToken cancellationToken = default
    )
    {
        var command = new CreateDeviceCommand()
        {
            Data = request,
            UserId = 1, // TODO: Get from claims
            EnterpriseId = 1 // TODO: Get from claims
        };

        return await ExecuteCommand<CreateDeviceCommand, DeviceDto>(command);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateDeviceDto request,
        CancellationToken cancellationToken = default)
    {
        var command = new UpdateDeviceCommand()
        {
            Id = id,
            Data = request,
            UserId = 1, // TODO: Get from claims
        };

        return await ExecuteCommand<UpdateDeviceCommand, DeviceDto>(command);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken = default)
    {
        var command = new DeleteDeviceCommand()
        {
            Id = id,
            EnterpriseId = 1 // TODO: Get from claims
        };
        return await ExecuteCommand<DeleteDeviceCommand, bool>(command);
    }
}