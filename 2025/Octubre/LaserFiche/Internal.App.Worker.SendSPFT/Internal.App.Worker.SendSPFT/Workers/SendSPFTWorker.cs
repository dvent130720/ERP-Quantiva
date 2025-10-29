

namespace Internal.App.Worker.SendSPFT.Workers
{
    using global::SendSPFT.Application.Contracts.Services;

    using Microsoft.Extensions.Logging;
    public class SendSftpWorker(
        ILogger<SendSftpWorker> logger,
        ISendSftpService service,
        IConfiguration config
    ) : BackgroundService
    {
        #region Logs
        private static readonly Action<ILogger, Exception> LogWorkerStarted =
            LoggerMessage.Define(LogLevel.Information, new EventId(1, nameof(SendSftpWorker)),
                "SendSftpWorker iniciado.");

        private static readonly Action<ILogger, Exception> LogWorkerStopped =
            LoggerMessage.Define(LogLevel.Information, new EventId(2, nameof(SendSftpWorker)),
                "SendSftpWorker finalizado.");

        private static readonly Action<ILogger, Exception> LogWorkerFatalError =
            LoggerMessage.Define(LogLevel.Error, new EventId(3, nameof(SendSftpWorker)),
                "Error fatal en el worker.");
        #endregion

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            LogWorkerStarted(logger, null);


            var delaySeconds = int.TryParse(config["ExecutionDelaySeconds"], out var seconds) ? seconds : 60;

            using var timer = new PeriodicTimer(TimeSpan.FromSeconds(delaySeconds));

            try
            {
                
                while (await timer.WaitForNextTickAsync(stoppingToken))
                {
                    try
                    {
                        await service.ExecuteServiceAsync(stoppingToken);
                    }
                    catch (Exception ex)
                    {
                        LogWorkerFatalError(logger, ex);
                    }
                }
            }
            catch (OperationCanceledException)
            {

                LogWorkerStopped(logger, null);
            }
        }
    }
}

