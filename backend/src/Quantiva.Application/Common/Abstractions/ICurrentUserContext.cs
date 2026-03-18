namespace Quantiva.Application.Common.Abstractions;

public interface ICurrentUserContext
{
    string? Email { get; }
    string? UserId { get; }
    string? IpAddress { get; }
    bool IsAuthenticated { get; }
}
