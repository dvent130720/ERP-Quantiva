namespace Core.Web.Api.Empresas.Infrastructure.Persistence;

public sealed class DatabaseOptions
{
    public const string SectionName = "ConnectionStrings";
    public string Postgres { get; set; } = string.Empty;
}
