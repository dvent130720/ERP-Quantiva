using System.Text;
using System.Text.Json;
using Core.Web.Api.FacturaIA.Application.Interfaces;
using Core.Web.Api.FacturaIA.Domain.Entities;

namespace Core.Web.Api.FacturaIA.Api.Middleware;

public class AuditMiddleware
{
    private readonly RequestDelegate _next;

    public AuditMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IAuditoriaRepository auditoriaRepository, ICurrentUserService currentUserService)
    {
        var requestBody = await ReadRequestBodyAsync(context.Request);
        var originalResponseStream = context.Response.Body;

        await using var responseBody = new MemoryStream();
        context.Response.Body = responseBody;

        await _next(context);

        responseBody.Seek(0, SeekOrigin.Begin);
        var responseText = await new StreamReader(responseBody).ReadToEndAsync();
        responseBody.Seek(0, SeekOrigin.Begin);
        await responseBody.CopyToAsync(originalResponseStream);
        context.Response.Body = originalResponseStream;

        if (context.User.Identity?.IsAuthenticated == true)
        {
            var payload = JsonSerializer.Serialize(new { requestBody, responseText });
            await auditoriaRepository.AddAsync(new Auditoria
            {
                TenantId = currentUserService.GetTenantId(),
                Usuario = currentUserService.GetUserName(),
                Endpoint = $"{context.Request.Method} {context.Request.Path}",
                Accion = context.GetEndpoint()?.DisplayName ?? "http-request",
                Data = payload,
                Fecha = DateTime.UtcNow,
                StatusCode = context.Response.StatusCode
            });
        }
    }

    private static async Task<string> ReadRequestBodyAsync(HttpRequest request)
    {
        request.EnableBuffering();
        request.Body.Seek(0, SeekOrigin.Begin);
        using var reader = new StreamReader(request.Body, Encoding.UTF8, leaveOpen: true);
        var text = await reader.ReadToEndAsync();
        request.Body.Seek(0, SeekOrigin.Begin);
        return text;
    }
}
