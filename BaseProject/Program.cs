using BaseProject.Features.Devices.Dtos;
using BaseProject.Features.Devices.Services;
using BaseProject.Features.Devices.Validators;
using BaseProject.Infrastructure.Data;
using BaseProject.Infrastructure.Extentions;
using BaseProject.Infrastructure.Middleware;
using BaseProject.Infrastructure.Storage;
using BaseProject.Infrastructure.Storage.Provides;
using BaseProject.Shared.services;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.Extensions.FileProviders;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// ========================================================================
// PHASE 1: CORE FRAMEWORK SETUP
// ========================================================================

// Add controllers and API endpoints
builder.Services.AddControllers();

// Add Swagger/OpenAPI documentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ========================================================================
// PHASE 2: LOGGING CONFIGURATION (Serilog)
// ========================================================================

// Configure Serilog for structure logging
// Replaces Laravel's default logging system
builder.Host.UseSerilog((context, config) =>
{
    config
        // Console output
        .WriteTo.Console()
        // File rolling logs (daily)
        .WriteTo.File(
            "log/log-.txt",
            rollingInterval: RollingInterval.Day,
            outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz}] [{Level:u3}] {Message:lj}{NewLine}{Exception}]"
        )
        // Minimum level based on environment
        .MinimumLevel.Information()
        .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
        .Enrich.FromLogContext();
});

// ========================================================================
// PHASE 3: MEDIATR SETUP (CQRS Pattern)
// ========================================================================

// Register MediatR for command/query handlers
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));


// ========================================================================
// PHASE 4: AUTOMAPPER SETUP (DTO Mapping)
// ======================================================================== 

// Register AutoMapper for entity-to-DTO transformations
// Replaces Laravel Fractal transformers
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// ========================================================================
// PHASE 5: FLUENT VALIDATION SETUP
// ========================================================================

// Register FluentValidation
// Replaces Laravel Validate class with rules()
builder.Services.AddFluentValidationClientsideAdapters();

// Manual register validators (the won't audo-discover assembly scaning)
builder.Services.AddScoped<IValidator<CreateDeviceDto>, CreateDeviceValidator>();
builder.Services.AddScoped<IValidator<UpdateDeviceDto>, UpdateDeviceValidator>();

// ========================================================================
// PHASE 6: REDIS CACHE CONFIGURATION
// ========================================================================

// Register distributed cache using Redis
// Configuration: appsetings.json["Redis"]["Connection"]
builder.Services.AddStackExchangeRedisCache(options =>
{
    var redisConnection = builder.Configuration.GetConnectionString("Redis") ?? "localhost:6379";
    options.Configuration = redisConnection;
});

// ========================================================================
// INFRASTRUCTURE: DATABASE & REPOSITORIES
// ========================================================================

// Register Entity Framework Core DbContext
// Replace Laravel's Eloquent ORM
// Configuration in Infrastructure/Extensions/ServiceCollectionExtensions.cs
builder.Services.AddInfrastructureServices(builder.Configuration);

// Register generic repository for CRUD operations
// Replaces Laravel Model::create/update/delete
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

// ========================================================================
// INFRASTRUCTURE: FILE STORAGE CONFIGURATION
// ========================================================================

// Configure storage provider (FileSystem on MinIO)
// Replicates Laravel's configurable Storage facade
var storageProvider = builder.Configuration["Storage:Provider"] ?? "FileSystem";

if (storageProvider.Equals("MinIO", StringComparison.OrdinalIgnoreCase))
{
    // Use MinIO (S3-compatible) storage
    // Requires: dotnet add package Minio
    // Configuration: appsettings.json["Storage"]["MinIO"]
    builder.Services.AddScoped<IStorageProvider, MinIOStorageProvider>();
}
else
{
    // Use local file system storage
    // Configuration: appsettings.json["Storage"]["FileSystem"]
    builder.Services.AddScoped<IStorageProvider, FileSystemStorageProvider>();
}

// ========================================================================
// DOMAIN SERVICE: DEVICE DOMAIN
// ========================================================================

// Register Device domain services
// Custom business logic for device operations
builder.Services.AddScoped<IDeviceServices, DeviceService>();

// Register Attachment domain service
builder.Services.AddScoped<IAttachmentService, AttachmentService>();

// Note: Validators and other domain services follow the sam pattern
// Add them here as you build out more domains (Phase 5, 6, etc)


// ========================================================================
// BUILD APPLICATION
// ========================================================================
var app = builder.Build();

// ========================================================================
// MIDDLEWARE PIPELINE CONFIGURATION
// ========================================================================

// Configure the HTTP request pipeline
// 1. Exception handling middleware (replaces Laravel's exception handler)
app.UseMiddleware<ExceptionHandlingMiddleware>();

// 2. Swagger UI in development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// 3. HTTPS redirection
app.UseHttpsRedirection();

// 4. Serve static files from storage directory
// Allow public access to upload files
// Maps/storage/* to ./storage/* on disk
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "storage")),
    RequestPath = "/storage"
});

// 5. Routing
app.UseRouting();

// 6. Authorization (if needed in future phases
// app.UseAuthorization();

// ========================================================================
// ENDPOINT CONFIGURATION
// ========================================================================

// Map controller endpoints
// Routes are discoverd from [ApiController] + [Route] attributes
app.MapControllers();

// Map minimal endpoints if needed (can add health checks, etc)
// Example: app.MapGet("/health", () => "OK");

// ========================================================================
// START APPLICATIONS
// ========================================================================

// Run the application
app.Run();

// ========================================================================
// NOTES FOR FUTURE PHASES
// ========================================================================


/*
 * PHASE 5: ADD MORE DOMAINS
 * ===========================
 * As you add new domains (Position, Trip Alarm, Vehicle, etc),
 * follow this pattern:
 *
 * 1. Create validator is Featurs/{Domain}/Validators/
 * 2. Regis validator here:
 *      builder.Services.AddScoped<IValidator<YouDto>, YourValidator>();
 *
 * 3. Create domain services in Features/{Domain}/Servives/
 * 4. Register domain services here:
 *      builder.Services.AddScoped<IYourDomainService, YourDomainService>();
 *
 * PHASE 6: AUTHENTICATION & AUTHORIZATION
 * ===========================================
 *
 * When adding JWT/Azure AD authentication:
 *
 * 1. builder.Services.AddAuthentication(...)
 * 2. builder.Services.AddAuthorization(...)
 * 3. app.UseAuthentication()
 * 4. app.UseAuthorization()
 *
 * PHASE 7: BACKGROUND JOBS (HANGFIRE)
 * =======================================
 * When adding background job processing:
 *
 * 1. builder.Services.AddHangfire(config => ...)
 * 2. builder.Services.AddHangfireServer()
 * 3. app.UseHangfireDashboard()
 *
 * PHASE 8: REAL-TIME COMMUNICATION (SIGNALR)
 * =============================================
 * When adding Websocket support:
 * 1. builder.Servers.AddSignalR()
 * 2. app.UseRouting()
 * 3. app.UserEndpoints(endpoints => {
 *      endpoints.MapHub<YourHub>("/hub/path");
 * })
 *
 * PHASE 9: CACHING & DISTRIBUTED CACHING
 * ===========================================
 * Redis is already configured. To add caching:
 *
 * 1: Inject IDistributedCache into your handlers
 * 2. Use await cache.GetAsync/SetAsync(key, value)
 *
 * PHASE 10: METRICS & MONITORING (PROMETHEUS)
 * =============================================
 * When adding observability
 *
 * 1. builder.Servers.AddPrometheusExporter()
 * 2. Configure meter/tracer providers
 */