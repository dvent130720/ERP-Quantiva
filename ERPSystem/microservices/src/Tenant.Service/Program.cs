using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<TenantDbContext>(opt => opt.UseInMemoryDatabase("tenants"));

var app = builder.Build();

app.MapPost("/tenants", async (TenantCreateRequest request, TenantDbContext db) =>
{
    var key = ResolveMasterKey();
    var tenant = new Tenant
    {
        Id = Guid.NewGuid(),
        TenantId = request.TenantId,
        Ruc = request.Ruc,
        BusinessName = request.BusinessName,
        P12Encrypted = Encrypt(Convert.FromBase64String(request.P12Base64), key),
        P12PasswordEncrypted = Convert.ToBase64String(Encrypt(System.Text.Encoding.UTF8.GetBytes(request.P12Password), key)),
        SriEnvironment = request.SriEnvironment
    };

    db.Tenants.Add(tenant);
    await db.SaveChangesAsync();
    return Results.Created($"/tenants/{tenant.TenantId}", tenant);
});

app.MapGet("/tenants/{tenantId}", async (Guid tenantId, TenantDbContext db) =>
    await db.Tenants.FirstOrDefaultAsync(x => x.TenantId == tenantId) is { } tenant ? Results.Ok(tenant) : Results.NotFound());

app.MapGet("/tenants/{tenantId}/certificate", async (Guid tenantId, TenantDbContext db) =>
{
    var tenant = await db.Tenants.FirstOrDefaultAsync(x => x.TenantId == tenantId);
    return tenant is null
        ? Results.NotFound()
        : Results.Ok(new TenantCertificateResponse(tenant.TenantId, tenant.P12Encrypted, tenant.P12PasswordEncrypted));
});

app.Run();

public sealed class Tenant
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public string Ruc { get; set; } = default!;
    public string BusinessName { get; set; } = default!;
    public byte[] P12Encrypted { get; set; } = [];
    public string P12PasswordEncrypted { get; set; } = default!;
    public string SriEnvironment { get; set; } = "PRUEBAS";
}

public sealed record TenantCreateRequest(Guid TenantId, string Ruc, string BusinessName, string P12Base64, string P12Password, string SriEnvironment);
public sealed record TenantCertificateResponse(Guid TenantId, byte[] P12Encrypted, string P12PasswordEncrypted);

public sealed class TenantDbContext(DbContextOptions<TenantDbContext> options) : DbContext(options)
{
    public DbSet<Tenant> Tenants => Set<Tenant>();
}

static byte[] Encrypt(byte[] plain, byte[] key)
{
    var nonce = RandomNumberGenerator.GetBytes(12);
    var tag = new byte[16];
    var cipher = new byte[plain.Length];
    using var aes = new AesGcm(key, tagSizeInBytes: 16);
    aes.Encrypt(nonce, plain, cipher, tag);

    var payload = new byte[nonce.Length + tag.Length + cipher.Length];
    Buffer.BlockCopy(nonce, 0, payload, 0, nonce.Length);
    Buffer.BlockCopy(tag, 0, payload, nonce.Length, tag.Length);
    Buffer.BlockCopy(cipher, 0, payload, nonce.Length + tag.Length, cipher.Length);
    return payload;
}

static byte[] ResolveMasterKey()
{
    var base64 = Environment.GetEnvironmentVariable("P12_MASTER_KEY");
    if (string.IsNullOrWhiteSpace(base64))
    {
        throw new InvalidOperationException("P12_MASTER_KEY no configurada.");
    }

    var key = Convert.FromBase64String(base64);
    if (key.Length != 32)
    {
        throw new InvalidOperationException("P12_MASTER_KEY debe representar 32 bytes (AES-256).");
    }

    return key;
}
