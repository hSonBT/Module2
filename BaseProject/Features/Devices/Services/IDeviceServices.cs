using BaseProject.Features.Devices.Dtos;

namespace BaseProject.Features.Devices.Services;

public interface IDeviceServices
{
    Task<int> CountByEnterpriseAsync(int enterpriseId, CancellationToken cancellationToken);
    Task<IEnumerable<DeviceDto>> GetActiveDevicesAsync(int enterpriseId, CancellationToken cancellationToken);
    Task<bool> DeviceExistsAsync(int deviceId, int enterpriseId, CancellationToken cancellationToken);
}