namespace SendSPFT.CrossCutting.Mapper
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using Core.Customer.Web.Api.ConsultaDatosClientes.Business.Models;

    using Davivienda.Framework.Lib.VoucherNumber;

    using Microsoft.Extensions.Configuration;

    using SendSPFT.Business.Models.BD;
    using SendSPFT.Business.Models.SFTP;

    public class AttachmentMapper(IConfiguration config, IVoucherNumber voucher) : IAttachmentMapper
    {
        public FileProcDigDto MapToProcDig(Cliente datoscliente, TdAttachmentDto record, GetDatesSyb12ResponseDto dates)
        {
            var operacion = config.GetSection("Operation_SP").Value ?? string.Empty;
            var codigoSistema = config.GetValue<int>("System_Code");
            var num = voucher.GenerateVoucherNumber();
            var typeProduct = config.GetSection("Type_Product").Value;
            return new FileProcDigDto
            {
                Operacion = operacion,
                CodigoSistema = codigoSistema,
                IdSolicitud = num.NumeroComprobante,
                User = datoscliente.Identificacion,
                FechaProceso = (DateTime)dates.FechaProceso,
                FechaHora = (DateTime)dates.FechaHora,
                FechaEjecucion = (DateTime)dates.FechaDia,
                IdDoc = record.TdReportName,
                IdDocDef = num.NumeroComprobante,
                NombreCompleto = $"{datoscliente.PrimerNombre} {datoscliente.SegundoNombre}",
                Apellido1 = datoscliente.PrimerApellido,
                Apellido2 = datoscliente.SegundoApellido,
                RazonSocial = null,
                TipoIdent = record.TdTipoIdent,
                NumeroCedula = record.TdCedula,
                TipoPersona = datoscliente.TipoCliente,
                TipoProducto = typeProduct,
                NumeroOperacion = record.TdIdOperation,
                AnioPolitica = null,

                NumeroEnte = int.Parse(datoscliente.Ente, CultureInfo.InvariantCulture),

                TipoDocumento = record.TdFormat,
                Estado = record.TdOperacion,
                FechaCreditoMovil = null
            };
        }

       
    }

}
