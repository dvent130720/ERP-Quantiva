namespace BuildingBlocks.Contracts;

public interface IIntegrationEvent
{
    Guid EventId { get; }
    DateTimeOffset OccurredOn { get; }
    string TenantId { get; }
}

public sealed record InvoiceCreated(Guid InvoiceId, string TenantId, string AccessKey, Guid EventId, DateTimeOffset OccurredOn) : IIntegrationEvent;
public sealed record InvoiceSigned(Guid InvoiceId, string TenantId, Guid EventId, DateTimeOffset OccurredOn) : IIntegrationEvent;
public sealed record InvoiceSent(Guid InvoiceId, string TenantId, string SriTrackingId, Guid EventId, DateTimeOffset OccurredOn) : IIntegrationEvent;
public sealed record InvoiceAuthorized(Guid InvoiceId, string TenantId, string AuthorizationCode, Guid EventId, DateTimeOffset OccurredOn) : IIntegrationEvent;
public sealed record InvoiceError(Guid InvoiceId, string TenantId, string Reason, Guid EventId, DateTimeOffset OccurredOn) : IIntegrationEvent;
