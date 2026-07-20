using BaseProject.Features.Devices.Dtos;
using BaseProject.Features.Devices.Models;

namespace BaseProject.Features.Devices.Profile;

public class DeviceProfile : AutoMapper.Profile
{
    public DeviceProfile()
    {
        // Entity -> DTO
        CreateMap<Device, DeviceDto>();

        // CreateDeviceDto -> Entity
        CreateMap<CreateDeviceDto, Device>();

        // UpdateDeviceDto -> Entity
        CreateMap<DeviceDto, Device>()
            .ForAllMembers(opts
                => opts.Condition((src, dest, srcMember) => srcMember != null));
    }
}