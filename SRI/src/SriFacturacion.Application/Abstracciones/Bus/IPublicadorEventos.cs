namespace SriFacturacion.Application.Abstracciones.Bus;

public interface IPublicadorEventos
{
    Task PublicarAsync<T>(string cola, T mensaje, CancellationToken cancellationToken) where T : class;
}
