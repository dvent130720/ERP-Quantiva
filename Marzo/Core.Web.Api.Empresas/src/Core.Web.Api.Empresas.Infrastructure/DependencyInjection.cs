using System.Text;
using Core.Web.Api.Empresas.Application.Abstractions;
using Core.Web.Api.Empresas.Application.Services;
using Core.Web.Api.Empresas.Infrastructure.Authentication;
using Core.Web.Api.Empresas.Infrastructure.Persistence;
using Core.Web.Api.Empresas.Infrastructure.Repositories;
using Core.Web.Api.Empresas.Infrastructure.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using Prometheus;

namespace Core.Web.Api.Empresas.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddEmpresasInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<DatabaseOptions>(configuration.GetSection(DatabaseOptions.SectionName));
        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));

        services.AddSingleton<IDbConnectionFactory, NpgsqlConnectionFactory>();
        services.AddScoped<ICompanyRepository, CompanyRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<CompanyService>();
        services.AddScoped<AuthService>();
        services.AddSingleton<DatabaseInitializer>();
        services.AddSingleton<IPasswordHasher, Pbkdf2PasswordHasher>();
        services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                var jwtOptions = configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>() ?? new JwtOptions();
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtOptions.Issuer,
                    ValidAudience = jwtOptions.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Secret)),
                    ClockSkew = TimeSpan.Zero
                };
            });

        services.AddAuthorization();
        services.AddHealthChecks().AddNpgSql(
            connectionString: configuration.GetConnectionString("Postgres") ?? string.Empty,
            name: "postgres",
            failureStatus: HealthStatus.Unhealthy,
            tags: new[] { "db", "postgres" });

        services.AddHttpContextAccessor();
        services.AddSingleton(Metrics.CreateCounter("company_login_total", "Cantidad de logins exitosos.", new CounterConfiguration
        {
            LabelNames = new[] { "role" }
        }));

        return services;
    }
}
