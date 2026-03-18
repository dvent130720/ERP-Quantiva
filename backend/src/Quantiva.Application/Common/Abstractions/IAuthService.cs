using Quantiva.Application.DTOs;

namespace Quantiva.Application.Common.Abstractions;

public interface IAuthService
{
    Task<LoginResponseDto?> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken = default);
}
