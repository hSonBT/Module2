using AutoMapper;
using BaseProject.Features.Devices.Commands;
using BaseProject.Features.Devices.Dtos;
using BaseProject.Features.Devices.Models;
using BaseProject.Infrastructure.Data;
using FluentValidation;
using MediatR;

namespace BaseProject.Features.Devices.Handlers;

public class CreateDeviceCommandHandler : IRequestHandler<CreateDeviceCommand, DeviceDto>
{
    private readonly MyAppDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly IValidator<CreateDeviceDto> _validator;
    private readonly ILogger<CreateDeviceCommandHandler> _logger;

    public CreateDeviceCommandHandler(
        MyAppDbContext dbContext,
        IMapper mapper,
        IValidator<CreateDeviceDto> validator,
        ILogger<CreateDeviceCommandHandler> logger)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _validator = validator;
        _logger = logger;
    }

    public async Task<DeviceDto> Handle(CreateDeviceCommand request, CancellationToken cancellationToken)
    {
        // Step 1: Validate input (replaces Laravel validate())
        var validationResult = await _validator.ValidateAsync(request.Data, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.GroupBy(x => x.PropertyName)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(x => x.ErrorMessage).ToArray());
            throw new Core.Exceptions.ValidationException("Validation failed", errors);
        }

        // Set 2: Map DTO to Entity
        var device = _mapper.Map<Device>(request.Data);
        device.UserId = request.UserId;
        device.EnterpriseId = request.EnterpriseId ?? 1;
        device.CreatedAt = DateTime.UtcNow;

        // Step 3: Execute within transaction (replaces Laravel transaction())
        return await _dbContext.ExecuteInTransactionAsync(async ctx =>
        {
            // Save to database
            await ctx.Devices.AddAsync(device, cancellationToken);
            await ctx.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Device created: {DeviceId} by user {UserId}", device.Id, device.UserId);

            // Step 4: Return Dto
            return _mapper.Map<DeviceDto>(device);
        }, cancellationToken);
    }
}