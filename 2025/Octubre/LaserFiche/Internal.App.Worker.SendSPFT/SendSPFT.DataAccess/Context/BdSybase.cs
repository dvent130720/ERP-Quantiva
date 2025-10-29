namespace SendSPFT.DataAccess.Context
{
    using System;
    using System.Data;
    using System.Data.CData.Sybase;
    using System.Threading.Tasks;


    using Davivienda.Framework.Lib.Crypto;

    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;

    using SendSPFT.DataAccess.Contracts.Context;

    public class BdSybase : IDisposable, IBdSybase
    {
        private readonly string _cadenaConexion;
        private readonly ILogger<BdSybase> _log;
        private bool _disposed;



       
        private static readonly Action<ILogger<BdSybase>, string, Exception> _logErrorDelegate =
            LoggerMessage.Define<string>(
                LogLevel.Error,
                new EventId(1001, nameof(IsHealthyAsync)),
                "Data Access Error: Connection not Healthy. Exception: {Message}"
            );

        public BdSybase(IConfiguration configuration, ICrypto crypto, ILogger<BdSybase> log)
        {
            ArgumentNullException.ThrowIfNull(configuration);
            ArgumentNullException.ThrowIfNull(crypto);
            ArgumentNullException.ThrowIfNull(log);

            _log = log;

            
            string cadenaEncriptada = configuration.GetSection("ConnectionStringSybaseDefault").Value
                ?? throw new InvalidOperationException("ConnectionStringSybaseDefault no configurada.");

            _cadenaConexion = crypto.Decrypt(cadenaEncriptada);
        }


        #region Dispose Pattern

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            _disposed = true;
        }

        #endregion

        public async Task<SybaseConnection> CreateConnectionAsync()
        {
            var conexion = new SybaseConnection(_cadenaConexion);
            await conexion.OpenAsync();
            return conexion;
        }

        public async Task<bool> IsHealthyAsync()
        {
            try
            {
                await using var conexion = await CreateConnectionAsync();
                return conexion.State == ConnectionState.Open;
            }
            catch (SybaseException ex)
            {
                _logErrorDelegate(_log, ex.Message, ex);
                return false;
            }
        }
    }
}
