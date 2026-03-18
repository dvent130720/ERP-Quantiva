namespace Quantiva.Application.Common.Exceptions;

public sealed class TenantResolutionException(string message) : AppException(message);
