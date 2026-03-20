using FluentValidation;
using Microsoft.Extensions.Logging;
using SriFacturacion.Application.Abstracciones.Bus;
using SriFacturacion.Application.Abstracciones.Catalogos;
using SriFacturacion.Application.Abstracciones.Correo;
using SriFacturacion.Application.Abstracciones.Documentos;
using SriFacturacion.Application.Abstracciones.Facturacion;
using SriFacturacion.Application.Abstracciones.Infraestructura;
using SriFacturacion.Application.Abstracciones.Persistencia;
using SriFacturacion.Application.Abstracciones.Sri;
using SriFacturacion.Application.Configuracion;
using SriFacturacion.Application.DTOs;
using SriFacturacion.Domain.Entidades;
using SriFacturacion.Domain.Enumeradores;
using SriFacturacion.Domain.Eventos;

namespace SriFacturacion.Application.Facturas;

public sealed class ServicioFacturas : IServicioFacturas
{
    private readonly IValidator<CrearFacturaRequest> _validator;
    private readonly ICertificadoRepository _certificadoRepository;
    private readonly IFacturaRepository _facturaRepository;
    private readonly IProveedorCatalogos _proveedorCatalogos;
    private readonly IGeneradorXmlFactura _generadorXmlFactura;
    private readonly IValidadorXmlSri _validadorXmlSri;
    private readonly IGeneradorClaveAcceso _generadorClaveAcceso;
    private readonly IPublicadorEventos _publicadorEventos;
    private readonly IFirmadorXml _firmadorXml;
    private readonly ICifrador _cifrador;
    private readonly IClienteSriSoap _clienteSriSoap;
    private readonly IRepositorioArchivosFactura _repositorioArchivosFactura;
    private readonly IGeneradorPdfFactura _generadorPdfFactura;
    private readonly IEmailSender _emailSender;
    private readonly ILogger<ServicioFacturas> _logger;

    public ServicioFacturas(
        IValidator<CrearFacturaRequest> validator,
        ICertificadoRepository certificadoRepository,
        IFacturaRepository facturaRepository,
        IProveedorCatalogos proveedorCatalogos,
        IGeneradorXmlFactura generadorXmlFactura,
        IValidadorXmlSri validadorXmlSri,
        IGeneradorClaveAcceso generadorClaveAcceso,
        IPublicadorEventos publicadorEventos,
        IFirmadorXml firmadorXml,
        ICifrador cifrador,
        IClienteSriSoap clienteSriSoap,
        IRepositorioArchivosFactura repositorioArchivosFactura,
        IGeneradorPdfFactura generadorPdfFactura,
        IEmailSender emailSender,
        ILogger<ServicioFacturas> logger)
    {
        _validator = validator;
        _certificadoRepository = certificadoRepository;
        _facturaRepository = facturaRepository;
        _proveedorCatalogos = proveedorCatalogos;
        _generadorXmlFactura = generadorXmlFactura;
        _validadorXmlSri = validadorXmlSri;
        _generadorClaveAcceso = generadorClaveAcceso;
        _publicadorEventos = publicadorEventos;
        _firmadorXml = firmadorXml;
        _cifrador = cifrador;
        _clienteSriSoap = clienteSriSoap;
        _repositorioArchivosFactura = repositorioArchivosFactura;
        _generadorPdfFactura = generadorPdfFactura;
        _emailSender = emailSender;
        _logger = logger;
    }

    public async Task<CrearFacturaResponse> CrearAsync(CrearFacturaRequest request, string correlationId, CancellationToken cancellationToken)
    {
        await _validator.ValidateAndThrowAsync(request, cancellationToken);

        var certificado = await _certificadoRepository.ObtenerPorIdAsync(request.CertificadoId, cancellationToken)
            ?? throw new InvalidOperationException("No existe el certificado indicado.");

        if (!certificado.Activo || certificado.FechaExpiracion <= DateTimeOffset.UtcNow)
        {
            throw new InvalidOperationException("El certificado no se encuentra vigente.");
        }

        var factura = new Factura
        {
            NumeroDocumento = request.NumeroDocumento,
            RucEmisor = request.RucEmisor,
            RazonSocialEmisor = request.RazonSocialEmisor,
            CorreoCliente = request.CorreoCliente,
            IdentificacionCliente = request.IdentificacionCliente,
            NombreCliente = request.NombreCliente,
            Moneda = request.Moneda,
            CertificadoId = request.CertificadoId
        };

        foreach (var itemRequest in request.Items)
        {
            var tarifa = await _proveedorCatalogos.ObtenerTarifaIvaAsync(itemRequest.CodigoImpuesto, cancellationToken);
            var baseImponible = Math.Round((itemRequest.Cantidad * itemRequest.PrecioUnitario) - itemRequest.PorcentajeDescuento, 2, MidpointRounding.AwayFromZero);
            var impuesto = Math.Round(baseImponible * tarifa / 100m, 2, MidpointRounding.AwayFromZero);
            factura.Items.Add(new FacturaItem
            {
                FacturaId = factura.Id,
                CodigoPrincipal = itemRequest.CodigoPrincipal,
                Descripcion = itemRequest.Descripcion,
                Cantidad = itemRequest.Cantidad,
                PrecioUnitario = itemRequest.PrecioUnitario,
                PorcentajeDescuento = itemRequest.PorcentajeDescuento,
                BaseImponible = baseImponible,
                CodigoImpuesto = itemRequest.CodigoImpuesto,
                TarifaImpuesto = tarifa,
                ValorImpuesto = impuesto
            });
        }

        factura.Subtotal = factura.Items.Sum(x => x.BaseImponible);
        factura.Impuestos = factura.Items.Sum(x => x.ValorImpuesto);
        factura.Total = factura.Subtotal + factura.Impuestos;
        factura.ClaveAcceso = _generadorClaveAcceso.Generar(factura);
        factura.XmlGenerado = _generadorXmlFactura.Generar(factura);
        await _validadorXmlSri.ValidarAsync(factura.XmlGenerado, cancellationToken);

        await _facturaRepository.CrearAsync(factura, cancellationToken);
        await _publicadorEventos.PublicarAsync(ColasMensajeria.FacturasPendientes, new EventoFacturaCreada(factura.Id, correlationId, DateTimeOffset.UtcNow), cancellationToken);

        _logger.LogInformation("Factura {FacturaId} aceptada para procesamiento asíncrono", factura.Id);
        return new CrearFacturaResponse(factura.Id, factura.ClaveAcceso, EstadoFactura.Pendiente.ToString(), "Factura aceptada para procesamiento asíncrono");
    }

    public async Task<FacturaEstadoResponse?> ObtenerEstadoAsync(Guid facturaId, CancellationToken cancellationToken)
    {
        var factura = await _facturaRepository.ObtenerPorIdAsync(facturaId, cancellationToken);
        return factura is null
            ? null
            : new FacturaEstadoResponse(factura.Id, factura.Estado.ToString(), factura.MensajeEstado, factura.ClaveAcceso, factura.FechaAutorizacion);
    }

    public async Task ProcesarAsync(Guid facturaId, string correlationId, CancellationToken cancellationToken)
    {
        var factura = await _facturaRepository.ObtenerPorIdAsync(facturaId, cancellationToken)
            ?? throw new InvalidOperationException($"No existe la factura {facturaId}");
        var certificado = await _certificadoRepository.ObtenerPorIdAsync(factura.CertificadoId, cancellationToken)
            ?? throw new InvalidOperationException($"No existe el certificado {factura.CertificadoId}");

        factura.MarcarEnProceso();
        await _facturaRepository.ActualizarAsync(factura, cancellationToken);

        var clave = _cifrador.Descifrar(certificado.ClaveCifrada);
        factura.XmlFirmado = await _firmadorXml.FirmarAsync(factura.XmlGenerado, certificado, clave, cancellationToken);
        var xmlBase64 = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(factura.XmlFirmado));

        var recepcion = await _clienteSriSoap.EnviarComprobanteAsync(xmlBase64, cancellationToken);
        if (!recepcion.Exitoso)
        {
            factura.MarcarRechazada(recepcion.Mensaje);
            await _facturaRepository.ActualizarAsync(factura, cancellationToken);
            return;
        }

        factura.MarcarRecibida(recepcion.Mensaje);
        var autorizacion = await _clienteSriSoap.ConsultarAutorizacionAsync(factura.ClaveAcceso, cancellationToken);
        if (!autorizacion.Autorizado || string.IsNullOrWhiteSpace(autorizacion.XmlAutorizado) || autorizacion.FechaAutorizacion is null)
        {
            factura.MarcarRechazada(autorizacion.Mensaje);
            await _facturaRepository.ActualizarAsync(factura, cancellationToken);
            return;
        }

        await _repositorioArchivosFactura.GuardarXmlAutorizadoAsync(factura.Id, autorizacion.XmlAutorizado, cancellationToken);
        factura.MarcarAutorizada(autorizacion.XmlAutorizado, autorizacion.FechaAutorizacion.Value, autorizacion.Mensaje);
        await _facturaRepository.ActualizarAsync(factura, cancellationToken);

        await _publicadorEventos.PublicarAsync(
            ColasMensajeria.FacturasAutorizadas,
            new EventoFacturaAutorizada(factura.Id, correlationId, factura.ClaveAcceso, factura.CorreoCliente, autorizacion.FechaAutorizacion.Value),
            cancellationToken);
    }

    public async Task EnviarCorreoAsync(EventoFacturaAutorizadaDto evento, CancellationToken cancellationToken)
    {
        var factura = await _facturaRepository.ObtenerPorIdAsync(evento.FacturaId, cancellationToken)
            ?? throw new InvalidOperationException($"No existe la factura {evento.FacturaId}");

        if (string.IsNullOrWhiteSpace(factura.XmlAutorizado))
        {
            factura.XmlAutorizado = System.Text.Encoding.UTF8.GetString(await _repositorioArchivosFactura.ObtenerXmlAutorizadoAsync(factura.Id, cancellationToken));
        }

        var pdf = _generadorPdfFactura.Generar(factura);
        var xml = System.Text.Encoding.UTF8.GetBytes(factura.XmlAutorizado!);
        await _emailSender.EnviarFacturaAsync(
            factura.CorreoCliente,
            $"Factura electrónica {factura.NumeroDocumento}",
            $"<p>Su factura electrónica {factura.NumeroDocumento} ha sido autorizada por el SRI.</p><p>Clave de acceso: {factura.ClaveAcceso}</p>",
            pdf,
            xml,
            factura.ClaveAcceso,
            cancellationToken);

        _logger.LogInformation("Correo enviado para la factura {FacturaId}", factura.Id);
    }
}
