namespace SendSPFT.CrossCutting.Mapper
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using Core.Customer.Web.Api.ConsultaDatosClientes.Business.Models;

    using SendSPFT.Business.Models.BD;
    using SendSPFT.Business.Models.SFTP;

    public interface IAttachmentMapper
    {
        FileProcDigDto MapToProcDig(Cliente datoscliente, SftpUploadResult result, TdAttachmentDto record, GetDatesSyb12ResponseDto dates);
        
    }
}
