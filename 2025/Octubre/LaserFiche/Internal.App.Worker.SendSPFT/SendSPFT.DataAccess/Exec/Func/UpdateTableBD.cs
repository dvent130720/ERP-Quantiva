namespace SendSPFT.DataAccess.Exec.Func
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using Microsoft.Extensions.Logging;

    using Npgsql;

    using SendSPFT.Business.Models.BD;
    using SendSPFT.DataAccess.Contracts.Context;
    using SendSPFT.DataAccess.Contracts.Exec.Func;

    public class UpdateTableBD : IUpdateTableBD
    {
        private readonly ISendSftpContext _pgContext;
        private readonly ILogger<UpdateTableBD> _log;
        #region
    
        private static readonly Action<ILogger, int, Exception?> _logNoRowsUpdated =
            LoggerMessage.Define<int>(
                LogLevel.Warning,
                new EventId(1001, nameof(UpdateTableAsync)),
                "No se actualizó ningún record con td_id = {TdId}");

        private static readonly Action<ILogger, int, Exception?> _logUpdatedSuccessfully =
            LoggerMessage.Define<int>(
                LogLevel.Information,
                new EventId(1002, nameof(UpdateTableAsync)),
                "record actualizado correctamente. td_id = {TdId}");

        private static readonly Action<ILogger, int, Exception> _logNpgsqlError =
            LoggerMessage.Define<int>(
                LogLevel.Error,
                new EventId(1003, nameof(UpdateTableAsync)),
                "Error al ejecutar UPDATE en td_attachment para td_id = {TdId}");

        private static readonly Action<ILogger, Exception> _logUnexpectedError =
            LoggerMessage.Define(
                LogLevel.Error,
                new EventId(1004, nameof(UpdateTableAsync)),
                "Error inesperado al actualizar record en td_attachment");
        #endregion
        public UpdateTableBD(ISendSftpContext pgContext, ILogger<UpdateTableBD> log)
        {
            _pgContext = pgContext ?? throw new ArgumentNullException(nameof(pgContext));
            _log = log ?? throw new ArgumentNullException(nameof(log));
        }

        public async Task UpdateTableAsync(TdAttachmentDto record)
        {
            const string sql = @"
                UPDATE td_attachment
                SET 
                    td_operacion = @td_operacion,
                    td_operacion_fecha_envio = @td_operacion_fecha_envio
                WHERE td_id = @td_id;";

            try
            {
                await using var conn = await _pgContext.CreateConnectionAsync();
                await using var cmd = new NpgsqlCommand(sql, conn);

                
                cmd.Parameters.AddWithValue("@td_operacion", record.TdOperacion ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@td_operacion_fecha_envio", record.TdOperacionFechaEnvio ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@td_id", record.TdId);

                var rowsAffected = await cmd.ExecuteNonQueryAsync();

                if (rowsAffected == 0)
                {
                    _logNoRowsUpdated(_log, record.TdId, null);
                }
                else
                {
                    _logUpdatedSuccessfully(_log, record.TdId, null);
                }
            }
            catch (NpgsqlException ex)
            {
                _logNpgsqlError(_log, record.TdId, ex);
                throw; 
            }
            catch (Exception ex)
            {
                _logUnexpectedError(_log, ex);
                throw;
            }
        }
    }


}
