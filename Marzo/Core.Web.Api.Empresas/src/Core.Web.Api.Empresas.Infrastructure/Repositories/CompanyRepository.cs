using Core.Web.Api.Empresas.Application.Abstractions;
using Core.Web.Api.Empresas.Domain.Entities;
using Core.Web.Api.Empresas.Infrastructure.Persistence;
using Dapper;

namespace Core.Web.Api.Empresas.Infrastructure.Repositories;

public sealed class CompanyRepository : ICompanyRepository
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public CompanyRepository(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task<IReadOnlyCollection<Company>> GetAllAsync(CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT id, tax_id AS TaxId, legal_name AS LegalName, trade_name AS TradeName,
                   email, phone, address, is_active AS IsActive,
                   created_at_utc AS CreatedAtUtc, updated_at_utc AS UpdatedAtUtc
            FROM companies
            ORDER BY legal_name;
            """;

        using var connection = _dbConnectionFactory.CreateConnection();
        var companies = await connection.QueryAsync<Company>(new CommandDefinition(sql, cancellationToken: cancellationToken));
        return companies.ToArray();
    }

    public async Task<Company?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT id, tax_id AS TaxId, legal_name AS LegalName, trade_name AS TradeName,
                   email, phone, address, is_active AS IsActive,
                   created_at_utc AS CreatedAtUtc, updated_at_utc AS UpdatedAtUtc
            FROM companies
            WHERE id = @Id;
            """;

        using var connection = _dbConnectionFactory.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<Company>(
            new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken));
    }

    public async Task<Company?> GetByTaxIdAsync(string taxId, CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT id, tax_id AS TaxId, legal_name AS LegalName, trade_name AS TradeName,
                   email, phone, address, is_active AS IsActive,
                   created_at_utc AS CreatedAtUtc, updated_at_utc AS UpdatedAtUtc
            FROM companies
            WHERE tax_id = @TaxId;
            """;

        using var connection = _dbConnectionFactory.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<Company>(
            new CommandDefinition(sql, new { TaxId = taxId }, cancellationToken: cancellationToken));
    }

    public async Task<Company> CreateAsync(Company company, CancellationToken cancellationToken)
    {
        const string sql = """
            INSERT INTO companies (id, tax_id, legal_name, trade_name, email, phone, address, is_active, created_at_utc, updated_at_utc)
            VALUES (@Id, @TaxId, @LegalName, @TradeName, @Email, @Phone, @Address, @IsActive, @CreatedAtUtc, @UpdatedAtUtc);
            """;

        using var connection = _dbConnectionFactory.CreateConnection();
        await connection.ExecuteAsync(new CommandDefinition(sql, company, cancellationToken: cancellationToken));
        return company;
    }

    public async Task<Company> UpdateAsync(Company company, CancellationToken cancellationToken)
    {
        const string sql = """
            UPDATE companies
            SET tax_id = @TaxId,
                legal_name = @LegalName,
                trade_name = @TradeName,
                email = @Email,
                phone = @Phone,
                address = @Address,
                is_active = @IsActive,
                updated_at_utc = @UpdatedAtUtc
            WHERE id = @Id;
            """;

        using var connection = _dbConnectionFactory.CreateConnection();
        await connection.ExecuteAsync(new CommandDefinition(sql, company, cancellationToken: cancellationToken));
        return company;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        const string sql = "DELETE FROM companies WHERE id = @Id;";

        using var connection = _dbConnectionFactory.CreateConnection();
        var affected = await connection.ExecuteAsync(new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken));
        return affected > 0;
    }
}
