using AutoMapper;
using BaseProject.Features.Devices.Commands;
using BaseProject.Features.Devices.Dtos;
using BaseProject.Infrastructure.Data;
using FluentValidation;
using MediatR;

namespace BaseProject.Features.Devices.Handlers;

public class UpdateDeviceCommandHandler : IRequestHandler<UpdateDeviceCommand, DeviceDto>
{
    private readonly MyAppDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly IValidator<UpdateDeviceDto> _validator;
    private readonly ILogger<UpdateDeviceCommandHandler> _logger;

    public UpdateDeviceCommandHandler(
        MyAppDbContext dbContext,
        IMapper mapper,
        IValidator<UpdateDeviceDto> validator,
        ILogger<UpdateDeviceCommandHandler> logger)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _validator = validator;
        _logger = logger;
    }


    public async Task<DeviceDto> Handle(UpdateDeviceCommand request, CancellationToken cancellationToken)
    {
        // Step 1: Validate input
        var validationResult = await _validator.ValidateAsync(request.Data, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .GroupBy(x => x.PropertyName)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(x => x.ErrorMessage).ToArray()
                );
            throw new Core.Exceptions.ValidationException("Validation failed", errors);
        }

        // Step 2: Load device
        var device =
            await _dbContext.Devices.FindAsync(new object[] { request.Id }, cancellationToken: cancellationToken);
        if (device == null) throw new Core.Exceptions.NotFoundException($"Deivce with id {request.Id} not found");

        // Step 3: Update within transaction
        return await _dbContext.ExecuteInTransactionAsync(async ctx =>
        {
            _mapper.Map(request.Data, device);
            device.UpdatedAt = DateTime.UtcNow;

            ctx.Devices.Update(device);
            await ctx.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Device updated: {DeviceId}", device.Id);
            return _mapper.Map<DeviceDto>(device);
        }, cancellationToken);
    }
}