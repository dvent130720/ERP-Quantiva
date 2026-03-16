using AuthBackend.Application.Contracts;
using AuthBackend.Domain.Entities;
using AuthBackend.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

namespace AuthBackend.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AuthDbContext _db;
    public UserRepository(AuthDbContext db) => _db = db;

    public Task<User?> GetByEmailAsync(string email, CancellationToken ct = default)
        => _db.Users.FirstOrDefaultAsync(x => x.Email == email.Trim().ToLower(), ct);

    public Task<User?> GetByGoogleIdAsync(string googleId, CancellationToken ct = default)
        => _db.Users.FirstOrDefaultAsync(x => x.GoogleId == googleId, ct);

    public Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => _db.Users.FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task AddAsync(User user, CancellationToken ct = default)
    {
        _db.Users.Add(user);
        await _db.SaveChangesAsync(ct);
    }
}

public class RedisRefreshTokenRepository : IRefreshTokenRepository
{
    private readonly IDistributedCache _cache;

    public RedisRefreshTokenRepository(IDistributedCache cache) => _cache = cache;

    public async Task SaveAsync(RefreshToken token, CancellationToken ct = default)
    {
        var value = $"{token.UserId}|{token.ExpiresAt:O}";
        await _cache.SetStringAsync($"refresh:{token.Token}", value, new DistributedCacheEntryOptions
        {
            AbsoluteExpiration = token.ExpiresAt
        }, ct);
    }

    public async Task<RefreshToken?> GetAsync(string token, CancellationToken ct = default)
    {
        var value = await _cache.GetStringAsync($"refresh:{token}", ct);
        if (string.IsNullOrWhiteSpace(value)) return null;

        var parts = value.Split('|');
        return new RefreshToken
        {
            Token = token,
            UserId = Guid.Parse(parts[0]),
            ExpiresAt = DateTime.Parse(parts[1]).ToUniversalTime()
        };
    }

    public Task RevokeAsync(string token, CancellationToken ct = default)
        => _cache.RemoveAsync($"refresh:{token}", ct);
}
