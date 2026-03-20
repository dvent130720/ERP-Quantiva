using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SriFacturacion.Application.Abstracciones.Documentos;
using SriFacturacion.Domain.Entidades;

namespace SriFacturacion.Infrastructure.Servicios.Documentos;

public sealed class GeneradorPdfFacturaQuest : IGeneradorPdfFactura
{
    static GeneradorPdfFacturaQuest()
    {
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public byte[] Generar(Factura factura)
    {
        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Margin(25);
                page.Header().Text($"Factura electrónica {factura.NumeroDocumento}").SemiBold().FontSize(18);
                page.Content().Column(columna =>
                {
                    columna.Item().Text($"Cliente: {factura.NombreCliente}");
                    columna.Item().Text($"Identificación: {factura.IdentificacionCliente}");
                    columna.Item().Text($"Clave de acceso: {factura.ClaveAcceso}");
                    columna.Item().Table(tabla =>
                    {
                        tabla.ColumnsDefinition(columnas =>
                        {
                            columnas.RelativeColumn(3);
                            columnas.RelativeColumn();
                            columnas.RelativeColumn();
                            columnas.RelativeColumn();
                        });

                        tabla.Header(encabezado =>
                        {
                            encabezado.Cell().Text("Descripción").Bold();
                            encabezado.Cell().Text("Cant.").Bold();
                            encabezado.Cell().Text("P.Unit").Bold();
                            encabezado.Cell().Text("Total").Bold();
                        });

                        foreach (var item in factura.Items)
                        {
                            tabla.Cell().Text(item.Descripcion);
                            tabla.Cell().Text(item.Cantidad.ToString("F2"));
                            tabla.Cell().Text(item.PrecioUnitario.ToString("F2"));
                            tabla.Cell().Text(item.Total.ToString("F2"));
                        }
                    });

                    columna.Item().PaddingTop(10).Text($"Subtotal: {factura.Subtotal:F2}");
                    columna.Item().Text($"Impuestos: {factura.Impuestos:F2}");
                    columna.Item().Text($"Total: {factura.Total:F2}").Bold();
                });
                page.Footer().AlignCenter().Text(texto =>
                {
                    texto.Span("Generado por SriFacturacion");
                    texto.Span(" - ");
                    texto.Span(DateTimeOffset.UtcNow.ToString("u"));
                });
            });
        }).GeneratePdf();
    }
}
