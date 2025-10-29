
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SendSPFT.Application.Contracts.Services;
using SendSPFT.CrossCutting.Subservices;
using SendSPFT.DataAccess.Contracts.Context;
using SendSPFT.DataAccess.Contracts.Exec.Func;


namespace SendSPFT.Application.Services
{
    public sealed class SendSftpService(
        ILogger<SendSftpService> logger,
        ISelectRegistryBD selectRecord,
        IProcessRecord processRecord,
        IGetDatesSyb12 sybase,
        INewSftpFlow newSftpFlow) : ISendSftpService
    {

        # region Logs
        private static readonly Action<ILogger, string, Exception?> _logInfo =
            LoggerMessage.Define<string>(LogLevel.Information, new EventId(1, nameof(_logInfo)), "{Message}");

        private static readonly Action<ILogger, string, Exception?> _logWarning =
            LoggerMessage.Define<string>(LogLevel.Warning, new EventId(2, nameof(_logWarning)), "{Message}");

        private static readonly Action<ILogger, Exception?> _logError =
            LoggerMessage.Define(LogLevel.Error, new EventId(3, nameof(_logError)), "Error general en el servicio.");
        #endregion 

        public async Task ExecuteServiceAsync(CancellationToken stoppingToken)
        {
            _logInfo(logger, "Searching records in RDS...", null);

            try
            {
                var Records = await selectRecord.SelectRecordAsync();
                var dates = await sybase.ExecuteSpGetDates();
                if (Records == null || Records.Count == 0)
                {
                    _logInfo(logger, "No Records found in 'Registered' status.", null);
                    return;
                }

                foreach (var record in Records)
                {
                    await processRecord.ProcessRecordAsync(record, dates);
                }

                await newSftpFlow.ExecuteAsync(dates.FechaProceso, stoppingToken);


            }
            catch (Exception ex)
            {
                _logError(logger, ex);
            }
                        
        }

        
    }

}
