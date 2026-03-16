using AuthBackend.Application.Contracts;
using AuthBackend.Application.Models;
using AuthBackend.Domain.Entities;

namespace AuthBackend.Application.Services;

public class AuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IGoogleTokenValidator _googleTokenValidator;
    private readonly IStateStore _stateStore;

    public AuthService(IUserRepository userRepository, IRefreshTokenRepository refreshTokenRepository,
        IPasswordHasher passwordHasher, IJwtTokenService jwtTokenService,
        IGoogleTokenValidator googleTokenValidator, IStateStore stateStore)
    {
        _userRepository = userRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenService = jwtTokenService;
        _googleTokenValidator = googleTokenValidator;
        _stateStore = stateStore;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken ct = default)
    {
        var existing = await _userRepository.GetByEmailAsync(request.Email, ct);
        if (existing is not null) throw new InvalidOperationException("Email already exists");

        var user = new User
        {
            Email = request.Email.Trim().ToLowerInvariant(),
            PasswordHash = _passwordHasher.Hash(request.Password),
            Provider = "local"
        };

        await _userRepository.AddAsync(user, ct);
        return await BuildAuthResponseAsync(user, ct);
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken ct = default)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email, ct)
                   ?? throw new UnauthorizedAccessException("Invalid credentials");

        if (string.IsNullOrWhiteSpace(user.PasswordHash) || !_passwordHasher.Verify(request.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Invalid credentials");

        return await BuildAuthResponseAsync(user, ct);
    }

    public async Task<AuthResponse> LoginWithGoogleAsync(GoogleCallbackRequest request, CancellationToken ct = default)
    {
        if (!await _stateStore.IsValidStateAsync(request.State, ct))
            throw new UnauthorizedAccessException("Invalid OAuth state");

        var principal = await _googleTokenValidator.ValidateIdTokenAsync(request.IdToken, ct);

        var user = await _userRepository.GetByGoogleIdAsync(principal.Subject, ct)
                   ?? await _userRepository.GetByEmailAsync(principal.Email, ct);

        if (user is null)
        {
            user = new User
            {
                Email = principal.Email,
                GoogleId = principal.Subject,
                Provider = "google"
            };
            await _userRepository.AddAsync(user, ct);
        }
        else if (string.IsNullOrWhiteSpace(user.GoogleId))
        {
            user.GoogleId = principal.Subject;
            user.Provider = "google";
        }

        return await BuildAuthResponseAsync(user, ct);
    }

    public async Task<AuthResponse> RefreshAsync(RefreshRequest request, CancellationToken ct = default)
    {
        var existing = await _refreshTokenRepository.GetAsync(request.RefreshToken, ct)
            ?? throw new UnauthorizedAccessException("Invalid refresh token");

        if (existing.ExpiresAt <= DateTime.UtcNow)
            throw new UnauthorizedAccessException("Expired refresh token");

        var user = await _userRepository.GetByIdAsync(existing.UserId, ct)
            ?? throw new UnauthorizedAccessException("Invalid refresh token");

        await _refreshTokenRepository.RevokeAsync(existing.Token, ct);
        return await BuildAuthResponseAsync(user, ct);
    }

    public Task LogoutAsync(string refreshToken, CancellationToken ct = default)
        => _refreshTokenRepository.RevokeAsync(refreshToken, ct);

    public async Task<string> CreateStateAsync(CancellationToken ct = default)
    {
        var state = Convert.ToHexString(Guid.NewGuid().ToByteArray());
        await _stateStore.StoreStateAsync(state, TimeSpan.FromMinutes(10), ct);
        return state;
    }

    private async Task<AuthResponse> BuildAuthResponseAsync(User user, CancellationToken ct)
    {
        var accessToken = _jwtTokenService.GenerateAccessToken(user);
        var refreshToken = _jwtTokenService.GenerateRefreshToken();
        var expires = DateTime.UtcNow.AddDays(7);
        await _refreshTokenRepository.SaveAsync(new RefreshToken
        {
            UserId = user.Id,
            Token = refreshToken,
            ExpiresAt = expires
        }, ct);

        return new AuthResponse(accessToken, refreshToken, expires);
    }
}
