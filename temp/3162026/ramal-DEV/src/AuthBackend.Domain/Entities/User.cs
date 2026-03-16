namespace AuthBackend.Domain.Entities;

public class User
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Email { get; set; } = string.Empty;
    public string? PasswordHash { get; set; }
    public string? GoogleId { get; set; }
    public string Provider { get; set; } = "local";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
