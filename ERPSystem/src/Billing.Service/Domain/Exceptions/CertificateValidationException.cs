namespace Billing.Service.Domain.Exceptions;

public sealed class CertificateValidationException(string message) : Exception(message);
public sealed class CertificateDecryptionException(string message) : Exception(message);
public sealed class XmlSigningException(string message, Exception? inner = null) : Exception(message, inner);
