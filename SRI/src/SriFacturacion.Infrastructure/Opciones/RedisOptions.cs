namespace SriFacturacion.Infrastructure.Opciones;

public sealed class RedisOptions
{
    public const string Seccion = "Redis";
    public string CadenaConexion { get; set; } = "redis:6379";
}
