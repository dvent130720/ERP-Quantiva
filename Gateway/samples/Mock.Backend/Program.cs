var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

app.MapGet("/api/v1/auth/{**catchAll}", (HttpContext context, string? catchAll) =>
{
    var userId = context.Request.Headers["X-User-Id"].FirstOrDefault() ?? "anonymous";
    var traceId = context.Request.Headers["X-Trace-Id"].FirstOrDefault() ?? context.TraceIdentifier;

    return Results.Ok(new
    {
        service = "mock-backend",
        route = catchAll,
        userId,
        traceId
    });
});

app.MapGet("/health", () => Results.Ok(new { status = "healthy" }));

app.Run();
