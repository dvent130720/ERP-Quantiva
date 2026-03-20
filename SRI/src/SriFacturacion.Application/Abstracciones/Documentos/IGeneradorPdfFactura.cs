using SriFacturacion.Domain.Entidades;

namespace SriFacturacion.Application.Abstracciones.Documentos;

public interface IGeneradorPdfFactura
{
    byte[] Generar(Factura factura);
}
