using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace External.Web.Api.Gateway.Infrastructure.Auth;

public static class JwtAuthenticationExtensions
{
    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<JwtOptions>()
            .Bind(configuration.GetSection(JwtOptions.SectionName))
            .Validate(options => !string.IsNullOrWhiteSpace(options.Authority), "Authentication:Jwt:Authority is required.")
            .Validate(options => !string.IsNullOrWhiteSpace(options.Audience), "Authentication:Jwt:Audience is required.")
            .ValidateOnStart();

        services.AddOptions<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme)
            .Configure<IOptions<JwtOptions>>((options, jwtOptionsAccessor) =>
            {
                var jwtOptions = jwtOptionsAccessor.Value;
                options.Authority = jwtOptions.Authority;
                options.Audience = jwtOptions.Audience;
                options.RequireHttpsMetadata = jwtOptions.RequireHttpsMetadata;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = true,
                    ValidateIssuer = true,
                    NameClaimType = "name",
                    RoleClaimType = "role"
                };
            });

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer();

        services.AddAuthorizationBuilder()
            .AddPolicy("Authenticated", policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme);
            })
            .SetFallbackPolicy(new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                .Build());

        return services;
    }
}
