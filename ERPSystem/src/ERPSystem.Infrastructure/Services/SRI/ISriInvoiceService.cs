public interface ISriInvoiceService
{
    Task<string> GenerateAccessKey(Guid invoiceId);
    Task<string> GenerateXml(Guid invoiceId);
    Task<string> SignXml(string xml, string certificatePath, string password);
    Task<SriRecepcionResponse> SendToSri(string signedXml);
    Task<SriAutorizacionResponse> CheckAuthorization(string accessKey);
}

public record SriRecepcionResponse(
    bool Success,
    string? Message,
    string? State,
    List<string> Errors
);

public record SriAutorizacionResponse(
    bool Success,
    string? AuthorizationNumber,
    DateTime? AuthorizationDate,
    string? State,
    List<string> Errors
);