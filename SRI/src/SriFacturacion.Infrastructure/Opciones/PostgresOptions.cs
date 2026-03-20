namespace SriFacturacion.Infrastructure.Opciones;

public sealed class PostgresOptions
{
    public const string Seccion = "PostgreSql";
    public string CadenaConexion { get; set; } = string.Empty;
}
