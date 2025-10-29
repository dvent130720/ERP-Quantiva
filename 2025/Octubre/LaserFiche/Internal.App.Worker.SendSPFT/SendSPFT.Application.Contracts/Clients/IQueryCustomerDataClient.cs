namespace SendSPFT.Application.Contracts.Clients
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using Core.Customer.Web.Api.ConsultaDatosClientes.Business.Models;

    using SendSPFT.Business.Models.ApiQCD;
    using SendSPFT.Business.Models.BD;

    public interface IQueryCustomerDataClient
    {
        Task<Cliente> GetDataClient(TdAttachmentDto dto);
    }
}
