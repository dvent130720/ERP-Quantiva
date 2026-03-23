namespace Core.Web.Api.FacturaIA.Domain.Common;

public interface ITenantEntity
{
    Guid TenantId { get; set; }
}

public abstract class BaseEntity : ITenantEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
