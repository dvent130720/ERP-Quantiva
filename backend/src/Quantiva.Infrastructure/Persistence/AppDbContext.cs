using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Quantiva.Application.Common.Abstractions;
using Quantiva.Domain.Common;
using Quantiva.Domain.Entities;

namespace Quantiva.Infrastructure.Persistence;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options, ITenantContext tenantContext, ICurrentUserContext currentUserContext)
    : DbContext(options)
{
    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<AppUser> Users => Set<AppUser>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<AuditTrail> AuditTrails => Set<AuditTrail>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Tenant>(entity =>
        {
            entity.ToTable("tenants");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Name).HasMaxLength(120).IsRequired();
            entity.Property(x => x.Slug).HasMaxLength(80).IsRequired();
            entity.Property(x => x.PrimaryDomain).HasMaxLength(180);
            entity.Property(x => x.ContactEmail).HasMaxLength(180);
            entity.HasIndex(x => x.Slug).IsUnique();
            entity.HasIndex(x => x.PrimaryDomain).IsUnique();
        });

        modelBuilder.Entity<AppUser>(entity =>
        {
            entity.ToTable("users");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.FullName).HasMaxLength(140).IsRequired();
            entity.Property(x => x.Email).HasMaxLength(180).IsRequired();
            entity.Property(x => x.PasswordHash).HasMaxLength(500).IsRequired();
            entity.HasIndex(x => new { x.Email, x.TenantId }).IsUnique();
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.ToTable("customers");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Code).HasMaxLength(50).IsRequired();
            entity.Property(x => x.LegalName).HasMaxLength(180).IsRequired();
            entity.Property(x => x.ContactEmail).HasMaxLength(180).IsRequired();
            entity.Property(x => x.ContactPhone).HasMaxLength(40);
            entity.HasIndex(x => new { x.TenantId, x.Code }).IsUnique();
            entity.HasOne(x => x.Tenant).WithMany(x => x.Customers).HasForeignKey(x => x.TenantId);
            entity.HasQueryFilter(x => tenantContext.IsAvailable && x.TenantId == tenantContext.TenantId);
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.ToTable("products");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Sku).HasMaxLength(60).IsRequired();
            entity.Property(x => x.Name).HasMaxLength(140).IsRequired();
            entity.Property(x => x.Description).HasMaxLength(600);
            entity.Property(x => x.Price).HasColumnType("numeric(18,2)");
            entity.HasIndex(x => new { x.TenantId, x.Sku }).IsUnique();
            entity.HasOne(x => x.Tenant).WithMany(x => x.Products).HasForeignKey(x => x.TenantId);
            entity.HasQueryFilter(x => tenantContext.IsAvailable && x.TenantId == tenantContext.TenantId);
        });

        modelBuilder.Entity<AuditTrail>(entity =>
        {
            entity.ToTable("audit_trails");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.EntityName).HasMaxLength(120).IsRequired();
            entity.Property(x => x.EntityId).HasMaxLength(80).IsRequired();
            entity.Property(x => x.Action).HasMaxLength(20).IsRequired();
            entity.Property(x => x.UserEmail).HasMaxLength(180);
            entity.Property(x => x.SourceIp).HasMaxLength(90);
            entity.Property(x => x.ChangesJson).HasColumnType("jsonb").IsRequired();
            entity.HasIndex(x => new { x.TenantId, x.OccurredAtUtc });
        });
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        var audits = new List<AuditTrail>();

        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            if (entry.Entity is AuditTrail)
            {
                continue;
            }

            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAtUtc = now;
                entry.Entity.UpdatedAtUtc = now;
                entry.Entity.CreatedBy = currentUserContext.Email ?? "system";
                entry.Entity.UpdatedBy = currentUserContext.Email ?? "system";

                if (entry.Entity is TenantScopedEntity tenantEntity && tenantContext.TenantId.HasValue)
                {
                    tenantEntity.TenantId = tenantContext.TenantId.Value;
                }
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAtUtc = now;
                entry.Entity.UpdatedBy = currentUserContext.Email ?? "system";
            }

            if (entry.State is EntityState.Added or EntityState.Modified or EntityState.Deleted)
            {
                var action = entry.State switch
                {
                    EntityState.Added => "Created",
                    EntityState.Modified => "Updated",
                    EntityState.Deleted => "Deleted",
                    _ => "Unknown"
                };

                var changes = entry.Properties
                    .Where(x => entry.State == EntityState.Added || x.IsModified || entry.State == EntityState.Deleted)
                    .ToDictionary(
                        x => x.Metadata.Name,
                        x => new
                        {
                            Original = entry.State == EntityState.Added ? null : x.OriginalValue,
                            Current = entry.State == EntityState.Deleted ? null : x.CurrentValue
                        });

                audits.Add(new AuditTrail
                {
                    TenantId = entry.Entity is TenantScopedEntity scoped ? scoped.TenantId : tenantContext.TenantId,
                    EntityName = entry.Metadata.ClrType.Name,
                    EntityId = (entry.Property(nameof(BaseEntity.Id)).CurrentValue ?? entry.Property(nameof(BaseEntity.Id)).OriginalValue)?.ToString() ?? string.Empty,
                    Action = action,
                    UserEmail = currentUserContext.Email,
                    SourceIp = currentUserContext.IpAddress,
                    OccurredAtUtc = now,
                    ChangesJson = JsonSerializer.Serialize(changes),
                    CreatedAtUtc = now,
                    UpdatedAtUtc = now,
                    CreatedBy = currentUserContext.Email ?? "system",
                    UpdatedBy = currentUserContext.Email ?? "system"
                });
            }
        }

        if (audits.Count > 0)
        {
            AuditTrails.AddRange(audits);
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}
