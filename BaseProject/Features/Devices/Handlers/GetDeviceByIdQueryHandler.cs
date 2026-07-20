using AutoMapper;
using BaseProject.Core.Exceptions;
using BaseProject.Features.Devices.Dtos;
using BaseProject.Features.Devices.Queries;
using BaseProject.Infrastructure.Data;
using MediatR;

namespace BaseProject.Features.Devices.Handlers;

public class GetDeviceByIdQueryHandler : IRequestHandler<GetDevicesByIdQuery, DeviceDto>
{
    private readonly MyAppDbContext _dbContext;
    private readonly IMapper _mapper;

    public GetDeviceByIdQueryHandler(MyAppDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<DeviceDto> Handle(GetDevicesByIdQuery request, CancellationToken cancellationToken)
    {
        var device = await _dbContext.Devices.FindAsync(new Object[] { request.Id }, cancellationToken);

        if (device == null)
            throw new NotFoundException($"Device with id {request.Id} not found");

        return _mapper.Map<DeviceDto>(device);
    }
}