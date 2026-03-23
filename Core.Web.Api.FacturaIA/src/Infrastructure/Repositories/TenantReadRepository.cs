using System.Data;
using Core.Web.Api.FacturaIA.Domain.Entities;
using Dapper;
using Npgsql;
using Microsoft.Extensions.Configuration;

namespace Core.Web.Api.FacturaIA.Infrastructure.Repositories;

public class TenantReadRepository
{
    private readonly string _connectionString;

    public TenantReadRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("DefaultConnection no configurada.");
    }

    public async Task<IReadOnlyList<Tenant>> GetAllAsync()
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        const string sql = "select id, tenant_id as TenantId, created_at as CreatedAt, nombre, pais_codigo as PaisCodigo, time_zone as TimeZone from tenants";
        var result = await connection.QueryAsync<Tenant>(sql);
        return result.ToList();
    }
}
