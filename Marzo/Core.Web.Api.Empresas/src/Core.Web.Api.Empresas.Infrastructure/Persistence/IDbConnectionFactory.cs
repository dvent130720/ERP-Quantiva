using System.Data;

namespace Core.Web.Api.Empresas.Infrastructure.Persistence;

public interface IDbConnectionFactory
{
    IDbConnection CreateConnection();
}
