using System.Threading.Channels;

namespace ERPSystem.API.Services;

public interface IInvoiceDispatchQueue
{
    ValueTask EnqueueAsync(Guid invoiceId, CancellationToken cancellationToken = default);
    ValueTask<Guid> DequeueAsync(CancellationToken cancellationToken);
}

public sealed class InvoiceDispatchQueue : IInvoiceDispatchQueue
{
    private readonly Channel<Guid> _queue = Channel.CreateUnbounded<Guid>();

    public ValueTask EnqueueAsync(Guid invoiceId, CancellationToken cancellationToken = default)
        => _queue.Writer.WriteAsync(invoiceId, cancellationToken);

    public ValueTask<Guid> DequeueAsync(CancellationToken cancellationToken)
        => _queue.Reader.ReadAsync(cancellationToken);
}
