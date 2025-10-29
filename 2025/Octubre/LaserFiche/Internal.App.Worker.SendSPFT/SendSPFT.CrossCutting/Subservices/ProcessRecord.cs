#nullable enable

namespace SendSPFT.CrossCutting.Subservices
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using SendSPFT.Application.Contracts.Clients;
    using SendSPFT.Business.Models.BD;
    using SendSPFT.DataAccess.Contracts.Exec.Func;
    using SendSPFT.CrossCutting.Subservices.ErrorSftp;
    using SendSPFT.DataAccess.Contracts.Context;

    public class ProcessRecord(
        ILogger<ProcessRecord> logger,
        IQueryCustomerDataClient queryCustomerData,
        IUpdateTableBD rds,
        IFileProcessingService fileProcessing,
        IConfiguration config,
        IErrorFileService errorFileService) 
        : IProcessRecord
    {
        #region Logs
        private static readonly Action<ILogger, string, Exception?> _logInfo =
            LoggerMessage.Define<string>(
                LogLevel.Information,
                new EventId(1, nameof(_logInfo)),
                "{Message}");

        private static readonly Action<ILogger, string, Exception?> _logWarning =
            LoggerMessage.Define<string>(
                LogLevel.Warning,
                new EventId(2, nameof(_logWarning)),
                "{Message}");

        private static readonly Action<ILogger, Exception?> _logError =
            LoggerMessage.Define(
                LogLevel.Error,
                new EventId(3, nameof(_logError)),
                "Error general en el servicio.");
        #endregion

        public async Task ProcessRecordAsync(TdAttachmentDto record , GetDatesSyb12ResponseDto dates)
        {
            try
            {

                _logInfo(logger, $"Processing file : {record.TdCedula}", null);
                
                var customerData = await queryCustomerData.GetDataClient(record);
                if (customerData == null)
                {
                    _logWarning(logger, "Data API ConsultaDatosCliente is Empty.", null);
                    return;
                }

                await fileProcessing.ProcessAsync(record, customerData, dates);

                record.TdOperacion = config.GetSection("Operation_sent").Value ?? string.Empty;
                record.TdOperacionFechaEnvio = DateTime.Now;

                await rds.UpdateTableAsync(record);
            }
            catch (Exception ex)
            {
                _logError(logger, ex);

                try
                {
                    await errorFileService.UploadErrorAsync(ex, $"Processing file ID: {record.TdId}", record.TdCedula);
                }
                catch (Exception nestedEx)
                {
                    
                    logger.LogCritical(nestedEx, "Critical failure while trying to register the error file.");
                }
            }
        }
    }
}
