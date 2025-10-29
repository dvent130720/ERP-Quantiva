using System.Globalization;
using System.Text;
using System.Text.Json;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using Renci.SshNet;

using SendSPFT.Application.Contracts.Clients;
using SendSPFT.Business.Models.SFTP;

namespace SendSPFT.Application.Clients
{
    public class SftpFileUploaderClient : ISftpFileUploader
    {
        private readonly ILogger<SftpFileUploaderClient> _logger;
        private readonly SftpSettings _settings;

        #region Logs
        private static readonly Action<ILogger, string, Exception?> _logSuccess =
            LoggerMessage.Define<string>(LogLevel.Information, new EventId(1001, nameof(_logSuccess)),
                "File successfully uploaded to SFTP: {RemoteFilePath}");

        private static readonly Action<ILogger, Exception?> _logError =
            LoggerMessage.Define(LogLevel.Error, new EventId(1002, nameof(_logError)), "Error uploading file to SFTP");
        #endregion

        public SftpFileUploaderClient(ILogger<SftpFileUploaderClient> logger, IConfiguration configuration)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _settings = configuration.GetSection("SftpSettings").Get<SftpSettings>()
                        ?? throw new ArgumentException("SFTP settings not found in configuration");
        }

        public async Task<SftpUploadResult> UploadFileAsync(string format, string base64Content, string reportName)
        {
            Validate(format, base64Content);
            return await UploadFileInternalAsync(format, base64Content, reportName);
        }

        public async Task<SftpUploadResult> UploadFileInternalAsync(string format, string base64Content, string reportName)
        {
            var result = new SftpUploadResult();
            var tempFilePath = string.Empty;

            try
            {
                var fileBytes = Convert.FromBase64String(base64Content);

                format = format.ToLower(CultureInfo.InvariantCulture);

            
                tempFilePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}-{reportName}");
                await File.WriteAllBytesAsync(tempFilePath, fileBytes);

                using var sftpClient = new SftpClient(_settings.Host, _settings.Port, _settings.Username, _settings.Password);
                sftpClient.Connect();

                var remoteFilePath = Path.Combine(_settings.RemotePath, Path.GetFileName(tempFilePath));

                await Task.Run(() => sftpClient.UploadFile(File.OpenRead(tempFilePath), remoteFilePath));

             
                result.Success = await Task.Run(() => sftpClient.Exists(remoteFilePath));
                result.RemoteFilePath = remoteFilePath;

                sftpClient.Disconnect();

                _logSuccess(_logger, remoteFilePath, null);
            }
            catch (Exception ex)
            {
                _logError(_logger, ex);
                result.Success = false;
                result.ErrorMessage = ex.Message;

                try
                {
                    string errorFileName = $"Error_{Guid.NewGuid()}.txt";
                    string errorFilePath = Path.Combine(Path.GetTempPath(), errorFileName);
                    await File.WriteAllTextAsync(errorFilePath, $"Error uploading {format} file:\n{ex}");

                    using var sftpClient = new SftpClient(_settings.Host, _settings.Port, _settings.Username, _settings.Password);
                    sftpClient.Connect();

                    string errorRemotePath = Path.Combine(_settings.ErrorPath, errorFileName);
                    await Task.Run(() => sftpClient.UploadFile(File.OpenRead(errorFilePath), errorRemotePath));

                    sftpClient.Disconnect();
                }
                catch (Exception innerEx)
                {
                    _logError(_logger, innerEx);
                }
            }

            return result;
        }

        public static void Validate(string format, string base64Content)
        {
            if (string.IsNullOrWhiteSpace(format))
                throw new ArgumentException("Format cannot be empty", nameof(format));

            if (string.IsNullOrWhiteSpace(base64Content))
                throw new ArgumentException("Base64 content cannot be empty", nameof(base64Content));
        }
        public async Task<SftpUploadResult> UploadErrorFileAsync(string format, string base64Content, string reportName)
        {
            var result = new SftpUploadResult();
            var tempFilePath = string.Empty;

            try
            {
                var fileBytes = Convert.FromBase64String(base64Content);
                

                tempFilePath = Path.Combine(Path.GetTempPath(), $"{reportName}");
                await File.WriteAllBytesAsync(tempFilePath, fileBytes);

                using var sftpClient = new SftpClient(_settings.Host, _settings.Port, _settings.Username, _settings.Password);
                sftpClient.Connect();

               
                var remoteFilePath = Path.Combine(_settings.ErrorPath, Path.GetFileName(tempFilePath));

                await Task.Run(() => sftpClient.UploadFile(File.OpenRead(tempFilePath), remoteFilePath));

                result.Success = await Task.Run(() => sftpClient.Exists(remoteFilePath));
                result.RemoteFilePath = remoteFilePath;

                sftpClient.Disconnect();

                _logSuccess(_logger, remoteFilePath, null);
            }
            catch (Exception ex)
            {
                _logError(_logger, ex);
                result.Success = false;
                result.ErrorMessage = ex.Message;
            }

            return result;
        }

    }
}
