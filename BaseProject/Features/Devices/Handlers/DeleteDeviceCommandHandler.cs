using BaseProject.Features.Devices.Commands;
using BaseProject.Infrastructure.Data;
using MediatR;

namespace BaseProject.Features.Devices.Handlers;

public class DeleteDeviceCommandHandler : IRequestHandler<DeleteDeviceCommand, bool>
public class DeleteDeviceHandler: IRequestHandler<DeleteDeviceCommand, bool>
{
    private readonly MyAppDbContext _dbContext;
    private readonly ILogger<DeleteDeviceCommandHandler> _logger;

    public DeleteDeviceCommandHandler(
        MyAppDbContext dbContext,
        ILogger<DeleteDeviceCommandHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<bool> Handle(DeleteDeviceCommand request, CancellationToken cancellationToken)
    {
        var device = await _dbContext.Devices.FindAsync(new object[] { request.Id }, cancellationToken);
        if (device == null) throw new Core.Exceptions.NotFoundException($"Device with id {request.Id} not found");

        return await _dbContext.ExecuteInTransactionAsync(async ctx =>
        {
            ctx.Devices.Remove(device);
            await ctx.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Device deleted: {DeviceId}", device.Id);
            return true;
        }, cancellationToken);
    }
    private readonly ILogger<DeleteDeviceHandler> _logger;
}