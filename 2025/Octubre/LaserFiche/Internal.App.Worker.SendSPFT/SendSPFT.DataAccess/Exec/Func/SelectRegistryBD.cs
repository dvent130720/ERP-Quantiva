namespace SendSPFT.DataAccess.Exec.Func
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;

    using Npgsql;

    using SendSPFT.Business.Models.BD;
    using SendSPFT.DataAccess.Contracts.Context;
    using SendSPFT.DataAccess.Contracts.Exec.Func;

    public class SelectRegistryBD : ISelectRegistryBD
    {
        private readonly ISendSftpContext _pgContext;
        private readonly ILogger<SelectRegistryBD> _log;
        private readonly IConfiguration _configuration;

        
        private static readonly Action<ILogger<SelectRegistryBD>, string, Exception?> LogInfo =
            LoggerMessage.Define<string>(
                LogLevel.Information,
                new EventId(1000, nameof(SelectRegistryBD)),
                "Ejecutando SELECT para estado: {State}");

        private static readonly Action<ILogger<SelectRegistryBD>, int, Exception?> LogInfoCount =
            LoggerMessage.Define<int>(
                LogLevel.Information,
                new EventId(1002, nameof(SelectRegistryBD)),
                "Registros obtenidos: {Count}");

        private static readonly Action<ILogger<SelectRegistryBD>, string, Exception?> LogError =
            LoggerMessage.Define<string>(
                LogLevel.Error,
                new EventId(1001, nameof(SelectRegistryBD)),
                "Error ejecutando SELECT en td_attachment: {ErrorMessage}");

        public SelectRegistryBD(
            ISendSftpContext pgContext,
            ILogger<SelectRegistryBD> log,
            IConfiguration configuration)
        {
            _pgContext = pgContext ?? throw new ArgumentNullException(nameof(pgContext));
            _log = log ?? throw new ArgumentNullException(nameof(log));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public async Task<List<TdAttachmentDto>> SelectRecordAsync()
        {
            var result = new List<TdAttachmentDto>();
            var opRegistered = _configuration.GetValue<string>("Registered_Operation") ?? string.Empty;

            const string sql = @"
                SELECT td_id,
                       td_cedula,
                       td_tipo_ident,
                       td_id_operation,
                       td_modelo,
                       td_application_id,
                       td_domain_host,
                       td_report_id,
                       td_adj_id_operation,
                       td_format,
                       td_base64,
                       td_operacion,
                       td_operacion_fecha_registro,
                       td_operacion_fecha_envio,
                       td_reportname
                FROM td_attachment
                WHERE td_operacion = @State;
            ";

            try
            {
               
                LogInfo(_log, opRegistered, null);

                await using var conn = await _pgContext.CreateConnectionAsync();
                await using var cmd = new NpgsqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@State", opRegistered);

                await using var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    result.Add(MapReaderToDto(reader));
                }

              
                LogInfoCount(_log, result.Count, null);
            }
            catch (NpgsqlException ex)
            {
                LogError(_log, ex.Message, ex);
                throw;
            }

            return result;
        }

        public TdAttachmentDto MapReaderToDto(NpgsqlDataReader reader)
        {
            string GetStringSafe(int index) => reader.IsDBNull(index) ? string.Empty : reader.GetString(index);
            DateTime? GetDateTimeSafe(int index) => reader.IsDBNull(index) ? null : reader.GetDateTime(index);

            return new TdAttachmentDto
            {
                TdId = reader.GetInt32(0),
                TdCedula = GetStringSafe(1),
                TdTipoIdent = GetStringSafe(2),
                TdIdOperation = GetStringSafe(3),
                TdModelo = GetStringSafe(4),
                TdApplicationId = GetStringSafe(5),
                TdDomainHost = GetStringSafe(6),
                TdReportId = GetStringSafe(7),
                TdAdjIdOperation = GetStringSafe(8),
                TdFormat = GetStringSafe(9),
                TdBase64 = GetStringSafe(10),
                TdOperacion = GetStringSafe(11),
                TdOperacionFechaRegistro = GetDateTimeSafe(12),
                TdOperacionFechaEnvio = GetDateTimeSafe(13),
                TdReportName = GetStringSafe(14)
            };
        }
    }
}
