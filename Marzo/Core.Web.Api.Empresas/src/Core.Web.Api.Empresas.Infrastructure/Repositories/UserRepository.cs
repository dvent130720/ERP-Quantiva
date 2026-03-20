using Core.Web.Api.Empresas.Application.Abstractions;
using Core.Web.Api.Empresas.Domain.Entities;
using Core.Web.Api.Empresas.Infrastructure.Persistence;
using Dapper;

namespace Core.Web.Api.Empresas.Infrastructure.Repositories;

public sealed class UserRepository : IUserRepository
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public UserRepository(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task<ApplicationUser?> GetByUsernameAsync(string username, CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT id, username, password_hash AS PasswordHash, role, created_at_utc AS CreatedAtUtc
            FROM app_users
            WHERE username = @Username;
            """;

        using var connection = _dbConnectionFactory.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<ApplicationUser>(
            new CommandDefinition(sql, new { Username = username }, cancellationToken: cancellationToken));
    }
}
