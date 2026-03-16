using System.Net.Http.Json;
using AuthBackend.Application.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient("AuthApi", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Services:AuthApiBaseUrl"]!);
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/health", () => Results.Ok(new { status = "ok", service = "gateway" }));

app.MapPost("/api/auth/register", async (RegisterRequest request, IHttpClientFactory factory, CancellationToken ct) =>
{
    var http = factory.CreateClient("AuthApi");
    var response = await http.PostAsJsonAsync("auth/register", request, ct);
    var payload = await response.Content.ReadAsStringAsync(ct);
    return Results.Content(payload, "application/json", (int)response.StatusCode);
});

app.MapPost("/api/auth/login", async (LoginRequest request, IHttpClientFactory factory, CancellationToken ct) =>
{
    var http = factory.CreateClient("AuthApi");
    var response = await http.PostAsJsonAsync("auth/login", request, ct);
    var payload = await response.Content.ReadAsStringAsync(ct);
    return Results.Content(payload, "application/json", (int)response.StatusCode);
});

app.MapGet("/api/auth/google", async (IHttpClientFactory factory, CancellationToken ct) =>
{
    var http = factory.CreateClient("AuthApi");
    var response = await http.GetAsync("auth/google", ct);
    var payload = await response.Content.ReadAsStringAsync(ct);
    return Results.Content(payload, "application/json", (int)response.StatusCode);
});

app.MapGet("/api/auth/google/callback", async (string id_token, string state, IHttpClientFactory factory, CancellationToken ct) =>
{
    var http = factory.CreateClient("AuthApi");
    var uri = $"auth/google/callback?id_token={Uri.EscapeDataString(id_token)}&state={Uri.EscapeDataString(state)}";
    var response = await http.GetAsync(uri, ct);
    var payload = await response.Content.ReadAsStringAsync(ct);
    return Results.Content(payload, "application/json", (int)response.StatusCode);
});

app.MapPost("/api/auth/refresh", async (RefreshRequest request, IHttpClientFactory factory, CancellationToken ct) =>
{
    var http = factory.CreateClient("AuthApi");
    var response = await http.PostAsJsonAsync("auth/refresh", request, ct);
    var payload = await response.Content.ReadAsStringAsync(ct);
    return Results.Content(payload, "application/json", (int)response.StatusCode);
});

app.MapPost("/api/auth/logout", async (RefreshRequest request, HttpRequest httpRequest, IHttpClientFactory factory, CancellationToken ct) =>
{
    var http = factory.CreateClient("AuthApi");
    var outgoing = new HttpRequestMessage(HttpMethod.Post, "auth/logout")
    {
        Content = JsonContent.Create(request)
    };

    if (httpRequest.Headers.TryGetValue("Authorization", out var authHeader))
    {
        outgoing.Headers.TryAddWithoutValidation("Authorization", authHeader.ToString());
    }

    var response = await http.SendAsync(outgoing, ct);
    if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
    {
        return Results.NoContent();
    }

    var payload = await response.Content.ReadAsStringAsync(ct);
    return Results.Content(payload, "application/json", (int)response.StatusCode);
});

app.Run();
