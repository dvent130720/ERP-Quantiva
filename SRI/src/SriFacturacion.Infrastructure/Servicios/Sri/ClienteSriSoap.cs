using System.Text;
using System.Xml.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using SriFacturacion.Application.Abstracciones.Sri;
using SriFacturacion.Infrastructure.Opciones;

namespace SriFacturacion.Infrastructure.Servicios.Sri;

public sealed class ClienteSriSoap : IClienteSriSoap
{
    private readonly HttpClient _httpClient;
    private readonly SriSoapOptions _options;
    private readonly ILogger<ClienteSriSoap> _logger;

    public ClienteSriSoap(HttpClient httpClient, IOptions<SriSoapOptions> options, ILogger<ClienteSriSoap> logger)
    {
        _httpClient = httpClient;
        _options = options.Value;
        _logger = logger;
        _httpClient.Timeout = TimeSpan.FromSeconds(_options.TiempoEsperaSegundos);
    }

    public Task<RespuestaRecepcionSri> EnviarComprobanteAsync(string xmlBase64, CancellationToken cancellationToken)
    {
        return Policy.Handle<HttpRequestException>()
            .Or<TaskCanceledException>()
            .WaitAndRetryAsync(3, intento => TimeSpan.FromSeconds(Math.Pow(2, intento)),
                (exception, espera, intento, _) => _logger.LogWarning(exception, "Reintento técnico {Intento} a recepción SRI en {Espera}", intento, espera))
            .ExecuteAsync(async token =>
            {
                var sobre = $"<soapenv:Envelope xmlns:soapenv='http://schemas.xmlsoap.org/soap/envelope/' xmlns:ec='http://ec.gob.sri.ws.recepcion'><soapenv:Header/><soapenv:Body><ec:validarComprobante><xml>{xmlBase64}</xml></ec:validarComprobante></soapenv:Body></soapenv:Envelope>";
                using var request = new HttpRequestMessage(HttpMethod.Post, _options.UrlRecepcion)
                {
                    Content = new StringContent(sobre, Encoding.UTF8, "text/xml")
                };
                using var response = await _httpClient.SendAsync(request, token);
                var payload = await response.Content.ReadAsStringAsync(token);
                response.EnsureSuccessStatusCode();
                var documento = XDocument.Parse(payload);
                var estado = documento.Descendants().FirstOrDefault(x => x.Name.LocalName == "estado")?.Value ?? "RECIBIDA";
                var mensaje = documento.Descendants().FirstOrDefault(x => x.Name.LocalName == "mensaje")?.Value ?? estado;
                return new RespuestaRecepcionSri(!string.Equals(estado, "DEVUELTA", StringComparison.OrdinalIgnoreCase), estado, mensaje);
            }, cancellationToken);
    }

    public Task<RespuestaAutorizacionSri> ConsultarAutorizacionAsync(string claveAcceso, CancellationToken cancellationToken)
    {
        return Policy.Handle<HttpRequestException>()
            .Or<TaskCanceledException>()
            .WaitAndRetryAsync(3, intento => TimeSpan.FromSeconds(Math.Pow(2, intento)),
                (exception, espera, intento, _) => _logger.LogWarning(exception, "Reintento técnico {Intento} a autorización SRI en {Espera}", intento, espera))
            .ExecuteAsync(async token =>
            {
                var sobre = $"<soapenv:Envelope xmlns:soapenv='http://schemas.xmlsoap.org/soap/envelope/' xmlns:ec='http://ec.gob.sri.ws.autorizacion'><soapenv:Header/><soapenv:Body><ec:autorizacionComprobante><claveAccesoComprobante>{claveAcceso}</claveAccesoComprobante></ec:autorizacionComprobante></soapenv:Body></soapenv:Envelope>";
                using var request = new HttpRequestMessage(HttpMethod.Post, _options.UrlAutorizacion)
                {
                    Content = new StringContent(sobre, Encoding.UTF8, "text/xml")
                };
                using var response = await _httpClient.SendAsync(request, token);
                var payload = await response.Content.ReadAsStringAsync(token);
                response.EnsureSuccessStatusCode();
                var documento = XDocument.Parse(payload);
                var estado = documento.Descendants().FirstOrDefault(x => x.Name.LocalName == "estado")?.Value ?? "AUTORIZADO";
                var mensaje = documento.Descendants().FirstOrDefault(x => x.Name.LocalName == "mensaje")?.Value ?? estado;
                var xmlAutorizado = documento.Descendants().FirstOrDefault(x => x.Name.LocalName == "comprobante")?.Value;
                var fechaTexto = documento.Descendants().FirstOrDefault(x => x.Name.LocalName == "fechaAutorizacion")?.Value;
                var fecha = DateTimeOffset.TryParse(fechaTexto, out var fechaAutorizacion)
                    ? fechaAutorizacion
                    : DateTimeOffset.UtcNow;
                return new RespuestaAutorizacionSri(string.Equals(estado, "AUTORIZADO", StringComparison.OrdinalIgnoreCase), estado, mensaje, xmlAutorizado, fecha);
            }, cancellationToken);
    }
}
