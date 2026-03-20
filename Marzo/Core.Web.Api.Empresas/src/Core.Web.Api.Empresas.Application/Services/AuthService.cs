using Core.Web.Api.Empresas.Application.Abstractions;
using Core.Web.Api.Empresas.Application.DTOs;
using Microsoft.Extensions.Logging;

namespace Core.Web.Api.Empresas.Application.Services;

public sealed class AuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IJwtTokenGenerator jwtTokenGenerator,
        ILogger<AuthService> logger)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
        _logger = logger;
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
        {
            throw new UnauthorizedAccessException("Credenciales inválidas.");
        }

        var user = await _userRepository.GetByUsernameAsync(request.Username.Trim(), cancellationToken);
        if (user is null || !_passwordHasher.Verify(request.Password, user.PasswordHash))
        {
            _logger.LogWarning("Intento de login fallido para el usuario {Username}", request.Username);
            throw new UnauthorizedAccessException("Credenciales inválidas.");
        }

        var (token, expiresAtUtc) = _jwtTokenGenerator.Generate(user);

        _logger.LogInformation(
            "Login exitoso para el usuario {Username} con rol {Role}. Token expira en {ExpiresAtUtc}",
            user.Username,
            user.Role,
            expiresAtUtc);

        return new LoginResponse(token, expiresAtUtc, "Bearer", user.Role);
    }
}
