// ============================================
// src/ERPSystem.Infrastructure/Persistence/ApplicationDbContext.cs
// ============================================
using Microsoft.EntityFrameworkCore;
using ERPSystem.Domain.Entities.Tenants;
using ERPSystem.Domain.Entities.Users;
using ERPSystem.Domain.Entities.Customers;
using ERPSystem.Domain.Entities.Products;
using ERPSystem.Domain.Entities.Invoices;

namespace ERPSystem.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Tenant> Tenants { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Invoice> Invoices { get; set; }
    public DbSet<InvoiceDetail> InvoiceDetails { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configuración Tenant
        modelBuilder.Entity<Tenant>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.CompanyName).HasMaxLength(255).IsRequired();
            entity.Property(e => e.Ruc).HasMaxLength(13).IsRequired();
            entity.HasIndex(e => e.Ruc).IsUnique();
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.Phone).HasMaxLength(20);
        });

        // Configuración User
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Username).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Email).HasMaxLength(255).IsRequired();
            entity.HasIndex(e => new { e.TenantId, e.Username }).IsUnique();
            entity.HasIndex(e => new { e.TenantId, e.Email }).IsUnique();
            
            entity.HasOne(e => e.Tenant)
                .WithMany(t => t.Users)
                .HasForeignKey(e => e.TenantId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configuración Customer
        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.IdentificationType).HasMaxLength(10).IsRequired();
            entity.Property(e => e.Identification).HasMaxLength(13).IsRequired();
            entity.Property(e => e.Name).HasMaxLength(255).IsRequired();
            entity.HasIndex(e => new { e.TenantId, e.Identification }).IsUnique();
            
            entity.HasOne(e => e.Tenant)
                .WithMany(t => t.Customers)
                .HasForeignKey(e => e.TenantId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configuración Product
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Code).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Name).HasMaxLength(255).IsRequired();
            entity.Property(e => e.Price).HasPrecision(18, 2);
            entity.Property(e => e.Cost).HasPrecision(18, 2);
            entity.Property(e => e.TaxPercentage).HasPrecision(5, 2);
            entity.HasIndex(e => new { e.TenantId, e.Code }).IsUnique();
            
            entity.HasOne(e => e.Tenant)
                .WithMany(t => t.Products)
                .HasForeignKey(e => e.TenantId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configuración Invoice
        modelBuilder.Entity<Invoice>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.EstablishmentCode).HasMaxLength(3).IsRequired();
            entity.Property(e => e.EmissionPointCode).HasMaxLength(3).IsRequired();
            entity.Property(e => e.SequentialNumber).HasMaxLength(9).IsRequired();
            entity.Property(e => e.FullNumber).HasMaxLength(17).IsRequired();
            entity.Property(e => e.AccessKey).HasMaxLength(49);
            entity.Property(e => e.Status).HasMaxLength(20).IsRequired();
            
            entity.Property(e => e.Subtotal).HasPrecision(18, 2);
            entity.Property(e => e.TaxAmount).HasPrecision(18, 2);
            entity.Property(e => e.Discount).HasPrecision(18, 2);
            entity.Property(e => e.Total).HasPrecision(18, 2);
            
            entity.HasIndex(e => new { e.TenantId, e.FullNumber }).IsUnique();
            entity.HasIndex(e => e.AccessKey).IsUnique();
            
            entity.HasOne(e => e.Tenant)
                .WithMany(t => t.Invoices)
                .HasForeignKey(e => e.TenantId)
                .OnDelete(DeleteBehavior.Cascade);
                
            entity.HasOne(e => e.Customer)
                .WithMany(c => c.Invoices)
                .HasForeignKey(e => e.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Configuración InvoiceDetail
        modelBuilder.Entity<InvoiceDetail>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ProductCode).HasMaxLength(50).IsRequired();
            entity.Property(e => e.ProductName).HasMaxLength(255).IsRequired();
            
            entity.Property(e => e.Quantity).HasPrecision(18, 4);
            entity.Property(e => e.UnitPrice).HasPrecision(18, 2);
            entity.Property(e => e.Discount).HasPrecision(18, 2);
            entity.Property(e => e.Subtotal).HasPrecision(18, 2);
            entity.Property(e => e.TaxPercentage).HasPrecision(5, 2);
            entity.Property(e => e.TaxAmount).HasPrecision(18, 2);
            entity.Property(e => e.Total).HasPrecision(18, 2);
            
            entity.HasOne(e => e.Invoice)
                .WithMany(i => i.Details)
                .HasForeignKey(e => e.InvoiceId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Datos de prueba (Seed)
        SeedData(modelBuilder);
    }

    private void SeedData(ModelBuilder modelBuilder)
    {
        // Tenant de prueba
        var tenantId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        modelBuilder.Entity<Tenant>().HasData(new Tenant
        {
            Id = tenantId,
            CompanyName = "Empresa Demo S.A.",
            CommercialName = "Demo Corp",
            Ruc = "1234567890001",
            Email = "demo@empresa.com",
            Phone = "0999999999",
            Address = "Av. Principal 123, Guayaquil",
            SriEnvironment = "PRUEBAS",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        });

        // Usuario admin de prueba
        var userId = Guid.Parse("22222222-2222-2222-2222-222222222222");
        modelBuilder.Entity<User>().HasData(new User
        {
            Id = userId,
            TenantId = tenantId,
            Username = "admin",
            Email = "admin@empresa.com",
            // Password: Admin123! (debe hashearse en producción)
            PasswordHash = "$2a$11$8K1p/a0dL3LkEZz7N.gP3.WVFPqW8Fg1LvXr/KxqHvQjYk8PCLG1i",
            FirstName = "Admin",
            LastName = "Sistema",
            Role = "ADMIN",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        });

        // Clientes de prueba
        var customer1Id = Guid.Parse("33333333-3333-3333-3333-333333333333");
        var customer2Id = Guid.Parse("44444444-4444-4444-4444-444444444444");
        
        modelBuilder.Entity<Customer>().HasData(
            new Customer
            {
                Id = customer1Id,
                TenantId = tenantId,
                IdentificationType = "CEDULA",
                Identification = "0912345678",
                Name = "Juan Pérez",
                Email = "juan.perez@email.com",
                Phone = "0991234567",
                Address = "Calle 1, Guayaquil",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new Customer
            {
                Id = customer2Id,
                TenantId = tenantId,
                IdentificationType = "RUC",
                Identification = "0987654321001",
                Name = "Corporación XYZ S.A.",
                Email = "contacto@xyz.com",
                Phone = "0987654321",
                Address = "Av. Principal 456, Guayaquil",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            }
        );

        // Productos de prueba
        var product1Id = Guid.Parse("55555555-5555-5555-5555-555555555555");
        var product2Id = Guid.Parse("66666666-6666-6666-6666-666666666666");
        
        modelBuilder.Entity<Product>().HasData(
            new Product
            {
                Id = product1Id,
                TenantId = tenantId,
                Code = "PROD001",
                Name = "Laptop Dell Inspiron",
                Description = "Laptop i7, 16GB RAM, 512GB SSD",
                Price = 1200.00m,
                Cost = 900.00m,
                TaxType = "IVA",
                TaxPercentage = 15m,
                StockQuantity = 10,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new Product
            {
                Id = product2Id,
                TenantId = tenantId,
                Code = "SERV001",
                Name = "Servicio de Consultoría",
                Description = "Consultoría en TI por hora",
                Price = 50.00m,
                Cost = 30.00m,
                TaxType = "IVA",
                TaxPercentage = 15m,
                StockQuantity = 0,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            }
        );
    }
}