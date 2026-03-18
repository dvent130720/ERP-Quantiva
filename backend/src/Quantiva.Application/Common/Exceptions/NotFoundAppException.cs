namespace Quantiva.Application.Common.Exceptions;

public sealed class NotFoundAppException(string message) : AppException(message);
