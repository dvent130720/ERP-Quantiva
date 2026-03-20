using SriFacturacion.Domain.Entidades;

namespace SriFacturacion.Application.Abstracciones.Documentos;

public interface IGeneradorXmlFactura
{
    string Generar(Factura factura);
}
