using System;
using System.Data;
using System.Threading.Tasks;

using Davivienda.Framework.Lib.Crypto;

using SendSPFT.DataAccess.Contracts.Context;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using Npgsql;

namespace SendSPFT.DataAccess.Context
{
    public class SendSftpContext : ISendSftpContext, IDisposable
    {
        private readonly string _cadenaConexion;
        private NpgsqlConnection? _conexion; 
        private readonly ILogger<SendSftpContext> _log;
        private bool _disposed;

        private static readonly Action<ILogger, string, Exception?> _logErrorConnection =
            LoggerMessage.Define<string>(
                LogLevel.Error,
                new EventId(1, nameof(_logErrorConnection)),
                "Data Access Error: Connection not Healthy - {Message}"
            );

        public SendSftpContext(IConfiguration configuration, ICrypto crypto, ILogger<SendSftpContext> log)
        {
            ArgumentNullException.ThrowIfNull(configuration);
            ArgumentNullException.ThrowIfNull(crypto);
            ArgumentNullException.ThrowIfNull(log);

            _log = log;

            string cadenaEncriptada = configuration.GetConnectionString("PostgresConnection")
                                      ?? throw new InvalidOperationException("PostgresConnection no configurada");

            _cadenaConexion = crypto.Decrypt(cadenaEncriptada);
        }

        public async Task<NpgsqlConnection> CreateConnectionAsync()
        {
            ThrowIfDisposed();

            _conexion = new NpgsqlConnection(_cadenaConexion);
            await _conexion.OpenAsync();
            return _conexion;
        }

        public async Task<bool> IsHealthyAsync()
        {
            ThrowIfDisposed();

            try
            {
                await using var conexion = await CreateConnectionAsync();
                return _conexion?.State == ConnectionState.Open;
            }
            catch (NpgsqlException ex)
            {
                _logErrorConnection(_log, ex.Message, ex);
                return false;
            }
        }

        protected void ThrowIfDisposed()
        {
            ObjectDisposedException.ThrowIf(_disposed, nameof(SendSftpContext));
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _conexion?.Dispose();
                }
                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}

