using System.Net.Mime;
using FluentValidation;
using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MultiTenant;
using Polly;
using Polly.Extensions.Http;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.Seq(builder.Configuration["Serilog:SeqUrl"] ?? "http://seq:5341")
    .CreateLogger();
builder.Host.UseSerilog();

builder.Services.AddCommonPlatform(builder.Configuration, "client-service");
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<CreateRecordHandler>());
builder.Services.AddValidatorsFromAssemblyContaining<CreateRecordValidator>();
builder.Services.AddDbContext<AppDbContext>(o => o.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((context, cfg) => cfg.Host(builder.Configuration.GetConnectionString("RabbitMq")));
});
builder.Services.AddStackExchangeRedisCache(o => o.Configuration = builder.Configuration.GetConnectionString("Redis"));
builder.Services.AddCommonHealthChecks(builder.Configuration);
builder.Services.AddHttpClient("internal")
    .AddPolicyHandler(HttpPolicyExtensions.HandleTransientHttpError().WaitAndRetryAsync(3, i => TimeSpan.FromMilliseconds(100 * i)))
    .AddPolicyHandler(HttpPolicyExtensions.HandleTransientHttpError().CircuitBreakerAsync(5, TimeSpan.FromSeconds(30)));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

app.UseCommonPlatform();
app.MapHealthChecks("/health");

app.MapGet("/api/client", async (AppDbContext db, ITenantProvider tenant) =>
{
    var data = await db.Records.Where(x => x.TenantId == tenant.Current.TenantId).OrderByDescending(x => x.CreatedAt).Take(50).ToListAsync();
    return Results.Ok(data);
}).RequireAuthorization();

app.MapPost("/api/client", async (CreateRecordCommand cmd, ISender sender) =>
{
    var result = await sender.Send(cmd);
    return Results.Created($"/api/client/{result.Id}", result);
}).Accepts<CreateRecordCommand>(MediaTypeNames.Application.Json).RequireAuthorization();

app.Run();

public sealed record CreateRecordCommand(string Name) : IRequest<RecordEntity>;

public sealed class CreateRecordValidator : AbstractValidator<CreateRecordCommand>
{
    public CreateRecordValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(120);
    }
}

public sealed class CreateRecordHandler(AppDbContext db, ITenantProvider tenant, IValidator<CreateRecordCommand> validator, IPublishEndpoint publish) : IRequestHandler<CreateRecordCommand, RecordEntity>
{
    public async Task<RecordEntity> Handle(CreateRecordCommand request, CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(request, cancellationToken);
        var entity = new RecordEntity
        {
            Name = request.Name.Trim(),
            TenantId = tenant.Current.TenantId,
            UpdatedAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow
        };
        db.Records.Add(entity);
        await db.SaveChangesAsync(cancellationToken);
        await publish.Publish(new DomainEvent($"client-serviceCreated", entity.Id, entity.TenantId), cancellationToken);
        return entity;
    }
}

public sealed class RecordEntity : TenantEntity
{
    public string Name { get; set; } = string.Empty;
}

public sealed record DomainEvent(string EventName, Guid AggregateId, string TenantId);

public sealed class AppDbContext(DbContextOptions<AppDbContext> options, ITenantProvider tenantProvider) : DbContext(options)
{
    public DbSet<RecordEntity> Records => Set<RecordEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<RecordEntity>(entity =>
        {
            entity.ToTable("records");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.TenantId).HasColumnName("tenant_id").HasMaxLength(64).IsRequired();
            entity.Property(x => x.Name).HasColumnName("name").HasMaxLength(120).IsRequired();
            entity.Property(x => x.CreatedAt).HasColumnName("created_at");
            entity.Property(x => x.UpdatedAt).HasColumnName("updated_at");
            entity.HasIndex(x => new { x.TenantId, x.CreatedAt });
            entity.HasQueryFilter(x => x.TenantId == tenantProvider.Current.TenantId);
        });
    }
}
