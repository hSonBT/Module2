using BaseProject.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BaseProject.Infrastructure.Extentions;

public static class ServiceCollectionExtentions
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Database
        services.AddDbContext<MyAppDbContext>(options => options.UseSqlServer(
            configuration.GetConnectionString("DefaultConnection"),
            sqlOptions => sqlOptions.MigrationsAssembly("BaseProject")));
        
        // Repository
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        
        return services;
    }
}