namespace ERPSystem.API.Filters;

public class ValidationFilter : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        // Aquí puedes agregar validaciones personalizadas
        // Por ahora solo pasamos al siguiente filtro
        return await next(context);
    }
}