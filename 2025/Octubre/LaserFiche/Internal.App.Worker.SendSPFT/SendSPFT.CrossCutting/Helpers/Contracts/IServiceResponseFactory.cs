namespace SendSPFT.CrossCutting.Helpers.Contracts
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using Davivienda.Framework.Services.CanonicalSignature;

    using SendSPFT.Bussiness.Models;

    public interface IServiceResponseFactory
    {
        ServiceResponse<T> Success<T>(T data, RequestContextHeaders ctx) where T : class;
        ServiceResponse<T> Fail<T>(string code, string message, RequestContextHeaders ctx) where T : class;
    }
}

