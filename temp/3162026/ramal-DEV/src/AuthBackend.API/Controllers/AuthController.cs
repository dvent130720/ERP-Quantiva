using AuthBackend.Application.Models;
using AuthBackend.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthBackend.API.Controllers;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;
    private readonly IConfiguration _configuration;

    public AuthController(AuthService authService, IConfiguration configuration)
    {
        _authService = authService;
        _configuration = configuration;
    }

    [HttpPost("register")]
    public Task<AuthResponse> Register([FromBody] RegisterRequest request, CancellationToken ct)
        => _authService.RegisterAsync(request, ct);

    [HttpPost("login")]
    public Task<AuthResponse> Login([FromBody] LoginRequest request, CancellationToken ct)
        => _authService.LoginAsync(request, ct);

    [HttpGet("google")]
    public async Task<IActionResult> Google(CancellationToken ct)
    {
        var state = await _authService.CreateStateAsync(ct);
        var clientId = _configuration["GoogleOAuth:ClientId"];
        var redirectUri = _configuration["GoogleOAuth:RedirectUri"];
        var url = $"https://accounts.google.com/o/oauth2/v2/auth?client_id={clientId}&redirect_uri={redirectUri}&response_type=code&scope=openid%20email%20profile&state={state}";
        return Ok(new { authorizationUrl = url, state });
    }

    [HttpGet("google/callback")]
    public Task<AuthResponse> GoogleCallback([FromQuery] string id_token, [FromQuery] string state, CancellationToken ct)
        => _authService.LoginWithGoogleAsync(new GoogleCallbackRequest(id_token, state), ct);

    [HttpPost("refresh")]
    public Task<AuthResponse> Refresh([FromBody] RefreshRequest request, CancellationToken ct)
        => _authService.RefreshAsync(request, ct);

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromBody] RefreshRequest request, CancellationToken ct)
    {
        await _authService.LogoutAsync(request.RefreshToken, ct);
        return NoContent();
    }
}
