using AutoMapper;
using BaseProject.Features.Devices.Dtos;
using BaseProject.Features.Devices.Queries;
using BaseProject.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BaseProject.Features.Devices.Handlers;

public class GetDevicesQueryHandler : IRequestHandler<GetDevicesQuery, IEnumerable<DeviceDto>> 
{
    private readonly MyAppDbContext _dbContext;
    private readonly IMapper _mapper;

    public GetDevicesQueryHandler(MyAppDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }
    
    public async Task<IEnumerable<DeviceDto>> Handle(GetDevicesQuery request, CancellationToken cancellationToken)
    {
        var query = _dbContext.Devices.AsQueryable();
        
        // Filter by enterprise (replaces Laravel scopeByEnterprise())
        if (request.EnterpriseId.HasValue)
            query = query.Where(d => d.EnterpriseId == request.EnterpriseId.Value);
        
        // Filter by active status
        if (request.ActiveOnly)
            query = query.Where(d => d.Active);
        
        // pagination
        var devices = await query
            .OrderByDescending(d => d.CreatedAt)
            .Skip(request.Skip)
            .Take(request.Take)
            .ToListAsync(cancellationToken);
        
        return _mapper.Map<IEnumerable<DeviceDto>>(devices);
    }
}