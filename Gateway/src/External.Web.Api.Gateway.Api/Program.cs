using External.Web.Api.Gateway.Api.Extensions;
using External.Web.Api.Gateway.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Host.AddInfrastructureLogging(builder.Configuration);
builder.AddGatewayApi();

var app = builder.Build();

app.UseGatewayApi();

app.Run();
