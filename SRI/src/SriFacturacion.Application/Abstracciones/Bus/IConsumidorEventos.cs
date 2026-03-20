namespace SriFacturacion.Application.Abstracciones.Bus;

public interface IConsumidorEventos
{
    Task IniciarConsumoAsync<T>(string cola, Func<T, string, Task<bool>> manejador, CancellationToken cancellationToken) where T : class;
}
