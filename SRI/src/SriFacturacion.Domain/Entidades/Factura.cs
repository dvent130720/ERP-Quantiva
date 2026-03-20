using SriFacturacion.Domain.Enumeradores;

namespace SriFacturacion.Domain.Entidades;

public sealed class Factura
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string NumeroDocumento { get; set; } = string.Empty;
    public string RucEmisor { get; set; } = string.Empty;
    public string RazonSocialEmisor { get; set; } = string.Empty;
    public string CorreoCliente { get; set; } = string.Empty;
    public string IdentificacionCliente { get; set; } = string.Empty;
    public string NombreCliente { get; set; } = string.Empty;
    public string Moneda { get; set; } = "USD";
    public decimal Subtotal { get; set; }
    public decimal Impuestos { get; set; }
    public decimal Total { get; set; }
    public string ClaveAcceso { get; set; } = string.Empty;
    public string XmlGenerado { get; set; } = string.Empty;
    public string? XmlFirmado { get; set; }
    public string? XmlAutorizado { get; set; }
    public EstadoFactura Estado { get; set; } = EstadoFactura.Pendiente;
    public string MensajeEstado { get; set; } = "Pendiente de procesamiento";
    public DateTimeOffset FechaCreacion { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? FechaAutorizacion { get; set; }
    public Guid CertificadoId { get; set; }
    public List<FacturaItem> Items { get; set; } = new();

    public void MarcarEnProceso() => Estado = EstadoFactura.EnProceso;

    public void MarcarRecibida(string mensaje)
    {
        Estado = EstadoFactura.RecibidaSri;
        MensajeEstado = mensaje;
    }

    public void MarcarAutorizada(string xmlAutorizado, DateTimeOffset fechaAutorizacion, string mensaje)
    {
        Estado = EstadoFactura.Autorizada;
        XmlAutorizado = xmlAutorizado;
        FechaAutorizacion = fechaAutorizacion;
        MensajeEstado = mensaje;
    }

    public void MarcarRechazada(string mensaje)
    {
        Estado = EstadoFactura.Rechazada;
        MensajeEstado = mensaje;
    }

    public void MarcarErrorTecnico(string mensaje)
    {
        Estado = EstadoFactura.ErrorTecnico;
        MensajeEstado = mensaje;
    }

    public void MarcarErrorCorreo(string mensaje)
    {
        Estado = EstadoFactura.ErrorCorreo;
        MensajeEstado = mensaje;
    }
}
