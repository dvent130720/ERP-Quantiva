namespace Billing.Service.Api;

public sealed record CreateInvoiceRequest(string TenantId, string Ruc, string Secuencial, decimal Total, string BuyerDocument, string BuyerName);
public sealed record InvoiceStatusUpdate(string Estado, string? XmlFirmado, string? XmlAutorizado, string? Error);
