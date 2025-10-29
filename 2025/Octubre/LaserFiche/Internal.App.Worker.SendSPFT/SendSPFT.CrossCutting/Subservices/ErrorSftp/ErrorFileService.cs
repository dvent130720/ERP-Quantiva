#nullable enable

namespace SendSPFT.CrossCutting.Subservices.ErrorSftp
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;

    using Microsoft.Extensions.Logging;

    using SendSPFT.Application.Contracts.Clients;

    public class ErrorFileService(ILogger<ErrorFileService> logger, ISftpFileUploader sftpUploader) : IErrorFileService
    {
        private static readonly Action<ILogger, string, Exception?> _logInfo =
            LoggerMessage.Define<string>(
                LogLevel.Information,
                new EventId(5001, nameof(_logInfo)),
                "Error file uploaded to SFTP successfully: {FileName}");

        private static readonly Action<ILogger, string, Exception?> _logUploadError =
            LoggerMessage.Define<string>(
                LogLevel.Error,
                new EventId(5002, nameof(_logUploadError)),
                "Failed to upload error file to SFTP for context: {Context}");

        private static readonly Action<ILogger, Exception?> _logBuildError =
            LoggerMessage.Define(
                LogLevel.Error,
                new EventId(5003, nameof(_logBuildError)),
                "Unexpected error while building or uploading the error report file.");

        private static readonly Action<ILogger, Exception?> _logCriticalFailure =
            LoggerMessage.Define(
                LogLevel.Critical,
                new EventId(5004, nameof(_logCriticalFailure)),
                "Critical failure while attempting to log or persist the error file.");

        public async Task UploadErrorAsync(Exception exception, string context, string cedula)
        {
            string fileName = $"{DateTime.UtcNow:yyyyMMdd_HHmmss}-{cedula}.txt";

            try
            {
                string content = BuildErrorContent(exception, context);
                string base64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(content));

                await sftpUploader.UploadErrorFileAsync("txt", base64, fileName);
                _logInfo(logger, fileName, null);
            }
            catch (Exception ex)
            {
                try
                {
                    _logUploadError(logger, context, ex);
                    string fallbackContent = $"[LOCAL LOGGING FAILURE]\nContext: {context}\nError: {ex}";
                    string localPath = Path.Combine(Path.GetTempPath(), $"LocalError_{Guid.NewGuid()}-.txt");
                    await File.WriteAllTextAsync(localPath, fallbackContent);
                    _logBuildError(logger, ex);
                }
                catch (Exception nestedEx)
                {
                    _logCriticalFailure(logger, nestedEx);
                }
            }
        }

        private static string BuildErrorContent(Exception ex, string context)
        {
            var sb = new StringBuilder();
            sb.AppendLine("--- ERROR REPORT ---");
            sb.AppendLine(string.Create(CultureInfo.InvariantCulture, $"Timestamp (UTC): {DateTime.UtcNow:O}"));
            sb.AppendLine(string.Create(CultureInfo.InvariantCulture, $"Context: {context}"));
            sb.AppendLine(string.Create(CultureInfo.InvariantCulture, $"Exception: {ex.Message}"));
            sb.AppendLine(string.Create(CultureInfo.InvariantCulture, $"StackTrace: {ex.StackTrace}"));

            if (ex.InnerException is not null)
            {
                sb.AppendLine();
                sb.AppendLine("--- INNER EXCEPTION ---");
                sb.AppendLine(ex.InnerException.ToString());
            }

            sb.AppendLine("----------------------");
            return sb.ToString();
        }
    }
}
