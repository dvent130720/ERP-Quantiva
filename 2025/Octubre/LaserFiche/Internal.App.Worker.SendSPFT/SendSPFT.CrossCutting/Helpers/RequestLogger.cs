namespace SendSPFT.CrossCutting.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Text.Json;
    using System.Threading.Tasks;

    using Davivienda.Framework.Lib.BitacoraLog.BitacoraLog;

    using SendSPFT.Bussiness.Models;
    using SendSPFT.CrossCutting.Helpers.Contracts;

    using Microsoft.Extensions.Logging;

    public class RequestLogger(IBitacoraLog bitacoraLog) : IRequestLogger
    {
        private const string Producto = "Auth";
        private const string Usuario = "Integration.Auth.Web.Api.SendSPFT";

        public void LogRequest(object body, RequestContextHeaders context, ILogger logger)
        {
            

            bitacoraLog.Registrar(CreateLog(context, "INICIANDO SERVICIO", "0", "00"));
        }

        public void LogSuccess(RequestContextHeaders context)
        {
            bitacoraLog.Registrar(CreateLog(context, "FINALIZADO", "0", "00"));
        }

        public void LogError(RequestContextHeaders context, Exception ex)
        {
            bitacoraLog.Registrar(CreateLog(context, "ERROR", "9999", ex.Message));
        }

        private static BitacoraLogDto CreateLog(RequestContextHeaders context, string estado, string codError, string descError)
        {
            return new BitacoraLogDto()
                .WithProducto(Producto)
                .WithIdServicio("Request")
                .WithIdTransaccion(context.TransactionId)
                .WithIdentificadorSesion(context.SessionId)
                .WithCanal(context.ChannelId)
                .WithUsuario(Usuario)
                .WithNumComprobante(context.VoucherNumber)
                .WithCodError(codError)
                .WithDescError(descError)
                .WithEstado(estado)
                .WithFecha(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture));
        }
    }
}

