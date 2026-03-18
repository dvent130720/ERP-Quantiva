using Quantiva.Domain.Common;

namespace Quantiva.Domain.Entities;

public sealed class AppUser : BaseEntity
{
    public Guid? TenantId { get; set; }
    public Tenant? Tenant { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public bool IsPlatformAdmin { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime? LastLoginAtUtc { get; set; }
}
