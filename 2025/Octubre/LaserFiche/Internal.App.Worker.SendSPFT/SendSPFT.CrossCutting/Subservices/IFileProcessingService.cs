namespace SendSPFT.CrossCutting.Subservices
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using Core.Customer.Web.Api.ConsultaDatosClientes.Business.Models;

    using SendSPFT.Business.Models.BD;

    public interface IFileProcessingService
    {
        Task ProcessAsync(TdAttachmentDto record, Cliente data, GetDatesSyb12ResponseDto dates);
    }
}
