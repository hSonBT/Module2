using BaseProject.Features.Devices.Commands;
using BaseProject.Infrastructure.Data;
using MediatR;

namespace BaseProject.Features.Devices.Handlers;

public class DeleteDeviceHandler: IRequestHandler<DeleteDeviceCommand, bool>
{
    private readonly MyAppDbContext _dbContext;
    private readonly ILogger<DeleteDeviceHandler> _logger;
}