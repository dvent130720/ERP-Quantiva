namespace SriFacturacion.Domain.ObjetosValor;

public sealed class ResultadoOperacion<T>
{
    public bool Exitoso { get; init; }
    public string Mensaje { get; init; } = string.Empty;
    public T? Datos { get; init; }

    public static ResultadoOperacion<T> Ok(T datos, string mensaje = "OK") => new() { Exitoso = true, Datos = datos, Mensaje = mensaje };
    public static ResultadoOperacion<T> Fallo(string mensaje) => new() { Exitoso = false, Mensaje = mensaje };
}
