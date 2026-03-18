using System.ComponentModel.DataAnnotations;

namespace Quantiva.Application.DTOs;

public sealed record CustomerDto(Guid Id, string Code, string LegalName, string ContactEmail, string? ContactPhone, string? Notes, bool IsActive, DateTime UpdatedAtUtc);

public sealed record UpsertCustomerDto(
    [property: Required, MaxLength(50)] string Code,
    [property: Required, MaxLength(180)] string LegalName,
    [property: Required, EmailAddress, MaxLength(180)] string ContactEmail,
    [property: MaxLength(40)] string? ContactPhone,
    [property: MaxLength(600)] string? Notes,
    bool IsActive);
