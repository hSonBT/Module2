using System.Net.Mail;
using BaseProject.Core.Abstractions;
using BaseProject.Features.Devices.Models;
using BaseProject.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace BaseProject.Infrastructure.Data;

public class MyAppDbContext : DbContext
{
    public MyAppDbContext(DbContextOptions<MyAppDbContext> options) : base(options)
    {
    }

    // DbSets will be added here for each entity
    // Example
    public DbSet<Device> Devices { get; set; }
    public DbSet<Shared.Models.Attachment> Attachments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // configure timestamp columns
        foreach (var entity in modelBuilder.Model.GetEntityTypes())
        {
            var createdAtProperty = entity.FindProperty("CreatedAt");
            if (createdAtProperty != null && createdAtProperty.ClrType == typeof(DateTime))
            {
                createdAtProperty.SetDefaultValueSql("GETUTCDATE()");
            }
        }

        // Configure Device table
        modelBuilder.Entity<Device>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Description).IsRequired().HasMaxLength(500);
            entity.Property(e => e.Protocol).HasMaxLength(50);
            entity.HasIndex(e => e.EnterpriseId);
            entity.HasIndex(e => e.UserId);
        });

        // Configure Attachment table
        modelBuilder.Entity<Shared.Models.Attachment>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FileName).IsRequired().HasMaxLength(255);
            entity.Property(e => e.MimeType).IsRequired().HasMaxLength(100);
            entity.Property(e => e.StoragePath).IsRequired().HasMaxLength(500);
            entity.Property(e => e.AttachableType).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Category).IsRequired().HasMaxLength(100);

            // Indexes for polymorphic queries
            entity.HasIndex(e => new { e.AttachableType, e.AttachableId });
            entity.HasIndex(e => e.EnterpriseId);
            entity.HasIndex(e => e.CreatedAt);
        });
    }

    /// <summary>
    /// Replaces Laravel`s transaction() - wraps operations in a transaction
    /// </summary>
    public async Task<T> ExecuteInTransactionAsync<T>(
        Func<MyAppDbContext, Task<T>> operation, CancellationToken cancellationToken = default)
    {
        using var transaction = await Database.BeginTransactionAsync(cancellationToken);
        try
        {
            var result = await operation(this);
            await transaction.CommitAsync(cancellationToken);
            return result;
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    /// <summary>
    /// Heper method for running queries with enterprises filtering
    /// Replaces Laravel's scoreByEnterprise()
    /// </summary>
    public IQueryable<T> QueryByEnterprise<T>(int enterpriseId) where T : BaseEntity
    {
        return Set<T>().Where(e => e.EnterpriseId == enterpriseId);
    }
}