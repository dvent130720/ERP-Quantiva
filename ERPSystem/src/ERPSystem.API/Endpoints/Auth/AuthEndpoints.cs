using Carter;
using ERPSystem.Infrastructure.Services.Identity;

namespace ERPSystem.API.Endpoints.Auth;

public class AuthEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/auth").WithTags("Authentication");

        // POST: Login
        group.MapPost("/login", async (LoginRequest request, IAuthService authService) =>
        {
            var response = await authService.Login(request.Username, request.Password);

            if (response == null)
            {
                return Results.Unauthorized();
            }

            return Results.Ok(response);
        })
        .WithName("Login")
        .Produces<AuthResponse>(200)
        .Produces(401)
        .AllowAnonymous();

        // POST: Register
        group.MapPost("/register", async (RegisterRequest request, IAuthService authService) =>
        {
            var response = await authService.Register(request);

            if (response == null)
            {
                return Results.BadRequest(new { message = "No se pudo crear el usuario. Verifica que el tenant exista y el username/email no estén en uso." });
            }

            return Results.Created($"/api/users/{response.UserId}", response);
        })
        .WithName("Register")
        .Produces<AuthResponse>(201)
        .Produces(400)
        .AllowAnonymous();

        // GET: Verificar token
        group.MapGet("/verify", (HttpContext context) =>
        {
            var userId = context.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var username = context.User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value;
            var email = context.User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
            var role = context.User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
            var tenantId = context.User.FindFirst("TenantId")?.Value;

            return Results.Ok(new
            {
                userId,
                username,
                email,
                role,
                tenantId,
                isAuthenticated = true
            });
        })
        .WithName("VerifyToken")
        .RequireAuthorization();
    }
}

public record LoginRequest(string Username, string Password);