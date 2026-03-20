using SriFacturacion.Domain.Entidades;

namespace SriFacturacion.Application.Abstracciones.Sri;

public interface IGeneradorClaveAcceso
{
    string Generar(Factura factura);
}
