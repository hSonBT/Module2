using AutoMapper;
using BaseProject.Features.Devices.Dtos;
using BaseProject.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BaseProject.Features.Devices.Services;

public class DeviceService : IDeviceServices
{
    private readonly MyAppDbContext _dbContext;
    private readonly IMapper _mapper;

    public DeviceService(MyAppDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<int> CountByEnterpriseAsync(int enterpriseId, CancellationToken cancellationToken)
        => await _dbContext.Devices
            .Where(d => d.EnterpriseId == enterpriseId)
            .CountAsync(cancellationToken);

    public async Task<IEnumerable<DeviceDto>> GetActiveDevicesAsync(int enterpriseId,
        CancellationToken cancellationToken)
    {
        var devices = await _dbContext.Devices
            .Where(d => d.EnterpriseId == enterpriseId && d.Active)
            .ToListAsync(cancellationToken);

        return _mapper.Map<IEnumerable<DeviceDto>>(devices);
    }

    public async Task<bool> DeviceExistsAsync(int deviceId, int enterpriseId, CancellationToken cancellationToken)
        => await _dbContext.Devices
            .AnyAsync(d => d.Id == deviceId && enterpriseId == d.EnterpriseId, cancellationToken);
}