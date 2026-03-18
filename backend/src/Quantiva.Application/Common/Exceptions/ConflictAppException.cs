namespace Quantiva.Application.Common.Exceptions;

public sealed class ConflictAppException(string message) : AppException(message);
