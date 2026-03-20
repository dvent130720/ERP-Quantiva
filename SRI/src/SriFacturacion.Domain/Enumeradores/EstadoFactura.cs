namespace SriFacturacion.Domain.Enumeradores;

public enum EstadoFactura
{
    Pendiente = 0,
    EnProceso = 1,
    RecibidaSri = 2,
    Autorizada = 3,
    Rechazada = 4,
    ErrorTecnico = 5,
    ErrorCorreo = 6
}
