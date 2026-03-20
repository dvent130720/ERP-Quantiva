using System.Globalization;
using System.Xml.Linq;
using SriFacturacion.Application.Abstracciones.Documentos;
using SriFacturacion.Domain.Entidades;

namespace SriFacturacion.Infrastructure.Servicios.Documentos;

public sealed class GeneradorXmlFacturaEcuador : IGeneradorXmlFactura
{
    public string Generar(Factura factura)
    {
        var documento = new XDocument(
            new XElement("factura",
                new XAttribute("id", "comprobante"),
                new XAttribute("version", "1.1.0"),
                new XElement("infoTributaria",
                    new XElement("razonSocial", factura.RazonSocialEmisor),
                    new XElement("ruc", factura.RucEmisor),
                    new XElement("claveAcceso", factura.ClaveAcceso),
                    new XElement("codDoc", "01"),
                    new XElement("estab", factura.NumeroDocumento[..3]),
                    new XElement("ptoEmi", factura.NumeroDocumento[4..7]),
                    new XElement("secuencial", factura.NumeroDocumento[^9..])),
                new XElement("infoFactura",
                    new XElement("fechaEmision", DateTimeOffset.UtcNow.ToString("dd/MM/yyyy")),
                    new XElement("dirEstablecimiento", "Matriz"),
                    new XElement("obligadoContabilidad", "SI"),
                    new XElement("tipoIdentificacionComprador", factura.IdentificacionCliente.Length == 13 ? "04" : "05"),
                    new XElement("razonSocialComprador", factura.NombreCliente),
                    new XElement("identificacionComprador", factura.IdentificacionCliente),
                    new XElement("totalSinImpuestos", factura.Subtotal.ToString("F2", CultureInfo.InvariantCulture)),
                    new XElement("importeTotal", factura.Total.ToString("F2", CultureInfo.InvariantCulture))),
                new XElement("detalles",
                    factura.Items.Select(item =>
                        new XElement("detalle",
                            new XElement("codigoPrincipal", item.CodigoPrincipal),
                            new XElement("descripcion", item.Descripcion),
                            new XElement("cantidad", item.Cantidad.ToString("F2", CultureInfo.InvariantCulture)),
                            new XElement("precioUnitario", item.PrecioUnitario.ToString("F2", CultureInfo.InvariantCulture)),
                            new XElement("descuento", item.PorcentajeDescuento.ToString("F2", CultureInfo.InvariantCulture)),
                            new XElement("precioTotalSinImpuesto", item.BaseImponible.ToString("F2", CultureInfo.InvariantCulture)),
                            new XElement("impuestos",
                                new XElement("impuesto",
                                    new XElement("codigo", "2"),
                                    new XElement("codigoPorcentaje", item.CodigoImpuesto),
                                    new XElement("tarifa", item.TarifaImpuesto.ToString("F2", CultureInfo.InvariantCulture)),
                                    new XElement("baseImponible", item.BaseImponible.ToString("F2", CultureInfo.InvariantCulture)),
                                    new XElement("valor", item.ValorImpuesto.ToString("F2", CultureInfo.InvariantCulture)))))))));

        return documento.Declaration is null ? documento.ToString(SaveOptions.DisableFormatting) : documento.ToString();
    }
}
