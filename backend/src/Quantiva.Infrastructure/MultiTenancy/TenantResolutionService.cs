using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Quantiva.Domain.Entities;
using Quantiva.Infrastructure.Persistence;

namespace Quantiva.Infrastructure.MultiTenancy;

public sealed class TenantResolutionService(AppDbContext dbContext, IOptions<TenantResolutionOptions> options)
{
    private readonly TenantResolutionOptions _options = options.Value;

    public async Task<Tenant?> ResolveAsync(string? headerSlug, string? host, CancellationToken cancellationToken = default)
    {
        var normalizedSlug = Normalize(headerSlug);
        if (!string.IsNullOrWhiteSpace(normalizedSlug))
        {
            return await dbContext.Tenants.AsNoTracking()
                .FirstOrDefaultAsync(x => x.IsActive && x.Slug == normalizedSlug, cancellationToken);
        }

        var normalizedHost = Normalize(host);
        if (string.IsNullOrWhiteSpace(normalizedHost) || _options.SharedHosts.Contains(normalizedHost))
        {
            return null;
        }

        var direct = await dbContext.Tenants.AsNoTracking()
            .FirstOrDefaultAsync(x => x.IsActive && x.PrimaryDomain == normalizedHost, cancellationToken);
        if (direct is not null)
        {
            return direct;
        }

        foreach (var baseDomain in _options.WildcardBaseDomains.Select(Normalize).Where(x => !string.IsNullOrWhiteSpace(x)))
        {
            var suffix = $".{baseDomain}";
            if (!normalizedHost.EndsWith(suffix, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            var slug = normalizedHost[..^suffix.Length];
            if (string.IsNullOrWhiteSpace(slug) || slug.Contains('.'))
            {
                continue;
            }

            return await dbContext.Tenants.AsNoTracking()
                .FirstOrDefaultAsync(x => x.IsActive && x.Slug == slug, cancellationToken);
        }

        return null;
    }

    private static string? Normalize(string? value) => value?.Trim().ToLowerInvariant();
}
