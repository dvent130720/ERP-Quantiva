using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddReverseProxy().LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));
builder.Services.AddRateLimiter(options =>
{
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(ctx =>
        RateLimitPartition.GetFixedWindowLimiter($"{ctx.Request.Headers["X-Tenant-Id"]}:{ctx.Connection.RemoteIpAddress}",
            _ => new FixedWindowRateLimiterOptions { PermitLimit = 50, Window = TimeSpan.FromSeconds(10), QueueLimit = 5 }));
});

var app = builder.Build();

app.Use(async (ctx, next) =>
{
    if (!ctx.Request.Headers.TryGetValue("X-Api-Key", out var apiKey) || string.IsNullOrWhiteSpace(apiKey))
    {
        ctx.Response.StatusCode = StatusCodes.Status401Unauthorized;
        await ctx.Response.WriteAsJsonAsync(new { error = "Missing API Key" });
        return;
    }
    await next();
});

app.UseRateLimiter();
app.MapReverseProxy();
app.Run();
