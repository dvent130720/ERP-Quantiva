using System;
using System.Data;
using System.Data.CData.Sybase;
using System.Globalization;
using System.Threading.Tasks;


using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using SendSPFT.Business.Models.BD;
using SendSPFT.DataAccess.Contracts.Context;

namespace SendSPFT.DataAccess.Exec.SP
{
    public class GetDatesSyb12 : IGetDatesSyb12
    {
        private readonly IBdSybase _bd;
        private readonly ILogger<GetDatesSyb12> _logger;
 

        private static readonly Action<ILogger, string, Exception?> LogDataMappingSuccessful =
            LoggerMessage.Define<string>(LogLevel.Information, new EventId(1002, nameof(GetDatesSyb12)),
                "Data mapping successful for {DataOrigin}");

        private static readonly Action<ILogger, string, Exception?> LogMappingError =
            LoggerMessage.Define<string>(LogLevel.Error, new EventId(2002, nameof(GetDatesSyb12)),
                "Error while mapping SP result for {DataOrigin}");

        private static readonly Action<ILogger, string, Exception?> LogSpError =
            LoggerMessage.Define<string>(LogLevel.Error, new EventId(2003, nameof(GetDatesSyb12)),
                "Error executing stored procedure for {DataOrigin}");

        public GetDatesSyb12(IBdSybase bd, ILogger<GetDatesSyb12> logger)
        {
            _bd = bd ?? throw new ArgumentNullException(nameof(bd));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
          
        }

        public async Task<GetDatesSyb12ResponseDto> ExecuteSpGetDates()
        {
            var dsResultado = new DataSet();
            var dataOrigin = "cob_interfaz";

            try
            {
                const string spName = "sp_int_api_cons_fecha_proceso";

                using var cnn = await _bd.CreateConnectionAsync();
               

                using var command = new SybaseCommand(spName, cnn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.Add(new SybaseParameter("@i_formato", 101));
                command.Parameters.Add(new SybaseParameter("@i_resulset", "S"));

                var adapter = new SybaseDataAdapter(command);
                adapter.Fill(dsResultado);

                return MapResult(dsResultado, dataOrigin);
            }
            catch (Exception ex)
            {
                LogSpError(_logger, dataOrigin, ex);
                throw new InvalidOperationException($"Error ejecutando SP para {dataOrigin}", ex);
            }
        }

        public GetDatesSyb12ResponseDto MapResult(DataSet ds, string dataOrigin)
        {
            if (ds == null || ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
                throw new InvalidOperationException("No data returned from stored procedure");

            try
            {
                var row = ds.Tables[0].Rows[0];
                var result = new GetDatesSyb12ResponseDto
                {
                    FechaProceso = row["fechaProceso"] != DBNull.Value ? Convert.ToDateTime(row["fechaProceso"], CultureInfo.InvariantCulture) : (DateTime?)null,
                    FechaDia = row["fechaDia"] != DBNull.Value ? Convert.ToDateTime(row["fechaDia"], CultureInfo.InvariantCulture) : (DateTime?)null,
                    FechaHora = row["fechaHora"] != DBNull.Value ? Convert.ToDateTime(row["fechaHora"], CultureInfo.InvariantCulture) : (DateTime?)null,
                    NumError = row["num_error"] != DBNull.Value ? Convert.ToInt32(row["num_error"], CultureInfo.InvariantCulture) : 0,
                    MsgError = row["msg_error"] != DBNull.Value ? Convert.ToString(row["msg_error"], CultureInfo.InvariantCulture) ?? string.Empty : string.Empty
                };

                LogDataMappingSuccessful(_logger, dataOrigin, null);
                return result;
            }
            catch (Exception ex)
            {
                LogMappingError(_logger, dataOrigin, ex);
                throw new InvalidOperationException($"Error mapeando resultado del SP para {dataOrigin}", ex);
            }
        }
    }
}
