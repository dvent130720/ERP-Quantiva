using Davivienda.Framework.Lib.Crypto;
using Davivienda.Framework.Services.ExceptionHandlerMiddleware.Middlewares.v1;
using Davivienda.Framework.Services.HeaderValidatorMiddleware;

using SendSPFT.CrossCutting.Config;
using SendSPFT.CrossCutting.Helpers;
using SendSPFT.Installers;

using Prometheus;

using Internal.App.Worker.SendSPFT.Workers;

using Serilog;

using ExceptionHandlerMiddleware = Davivienda.Framework.Services.ExceptionHandlerMiddleware.Middlewares.v1.ExceptionHandlerMiddleware;

var builder = WebApplication.CreateBuilder(args);


ConfigurationManager configuration = builder.Configuration;

builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                     .AddEnvironmentVariables()
                     .Build();

builder.Host.UseSerilog((context, loggerConfig) =>
    loggerConfig.ReadFrom.Configuration(context.Configuration));


builder.Services.AddSingleton<ICrypto, Crypto>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

Log.Information("Application Starting Up.");

builder.Services.Configure<AppConfiguration>(configuration.GetSection("ServicesConfig"));
builder.Services.RegisterServices(configuration);

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Internal.App.Worker.SendSPFT",
        Version = "v1",
        Description = "Lee desde postgres una cadena para luego decodificarla y enviar a SFTP",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "Diego Ventura",
            Email = "diego.ventura.proveedores@davivienda.com"
        }
    });
    options.OperationFilter<SwaggerHeaderFilter>();
});


builder.Services.AddHostedService<SendSftpWorker>();


builder.Services.AddHealthChecks();

 var app = builder.Build();


app.UseHttpsRedirection();
HealthCheckConfig.AddRegistration(app);

app.UseMetricServer();
app.UseHttpMetrics();
app.UseRouting();
app.UseMiddleware<ExceptionHandlerMiddleware>();
app.UseMiddleware<RequestHeaderValidationMiddleware>();
app.UseSerilogRequestLogging();


app.MapHealthChecks("/health");
app.MapControllers();


app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Internal.App.Worker.SendSFTP API v1");
    c.RoutePrefix = string.Empty;
});


await app.RunAsync();

