namespace Quantiva.Infrastructure.MultiTenancy;

public sealed class TenantResolutionOptions
{
    public const string SectionName = "TenantResolution";
    public List<string> SharedHosts { get; set; } = ["api.quantiva-solutions.com", "quantiva-solutions.com", "localhost"];
    public List<string> WildcardBaseDomains { get; set; } = ["quantiva-solutions.com", "localhost"];
}
