namespace Quantiva.Infrastructure.Security;

public sealed class JwtOptions
{
    public const string SectionName = "Jwt";
    public string Issuer { get; set; } = "quantiva-api";
    public string Audience { get; set; } = "quantiva-clients";
    public string SecretKey { get; set; } = "CHANGE_THIS_ULTRA_SECRET_32_CHAR_MINIMUM";
    public int ExpirationMinutes { get; set; } = 120;
}
