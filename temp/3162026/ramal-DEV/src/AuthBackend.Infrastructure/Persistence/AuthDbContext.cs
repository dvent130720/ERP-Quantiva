using AuthBackend.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AuthBackend.Infrastructure.Persistence;

public class AuthDbContext : DbContext
{
    public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Email).HasColumnName("email").IsRequired();
            entity.Property(x => x.PasswordHash).HasColumnName("password_hash");
            entity.Property(x => x.GoogleId).HasColumnName("google_id");
            entity.Property(x => x.Provider).HasColumnName("provider").IsRequired();
            entity.Property(x => x.CreatedAt).HasColumnName("created_at");
        });

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.ToTable("refresh_tokens");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.UserId).HasColumnName("user_id");
            entity.Property(x => x.Token).HasColumnName("token").IsRequired();
            entity.Property(x => x.ExpiresAt).HasColumnName("expires_at");
        });
    }
}
