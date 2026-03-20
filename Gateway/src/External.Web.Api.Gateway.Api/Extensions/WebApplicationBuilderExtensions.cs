using Microsoft.AspNetCore.HttpOverrides;

namespace External.Web.Api.Gateway.Api.Extensions;

public static class WebApplicationBuilderExtensions
{
    public static WebApplicationBuilder AddGatewayApi(this WebApplicationBuilder builder)
    {
        builder.Services.AddApi(builder.Configuration);
        builder.Services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            options.KnownNetworks.Clear();
            options.KnownProxies.Clear();
        });

        return builder;
    }
}
