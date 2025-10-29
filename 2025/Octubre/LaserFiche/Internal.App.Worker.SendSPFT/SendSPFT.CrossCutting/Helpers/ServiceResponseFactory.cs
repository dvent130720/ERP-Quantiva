namespace SendSPFT.CrossCutting.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using Davivienda.Framework.Services.CanonicalSignature;

    using SendSPFT.Bussiness.Models;
    using SendSPFT.CrossCutting.Helpers.Contracts;

    public  class ServiceResponseFactory : IServiceResponseFactory
    {
        public  ServiceResponse<T> Success<T>(T data, RequestContextHeaders ctx) where T : class
        {
            return new ServiceResponse<T>
            {
                Data = data,
                Succeeded = true,
                SessionId = ctx.SessionId,
                TransactionId = ctx.TransactionId,
                Errors = new List<ErrorDetail>
            {
                new ErrorDetail { Code = "0", Message = "OK" }
            }
            };
        }

        public  ServiceResponse<T> Fail<T>(string code, string message, RequestContextHeaders ctx) where T : class
        {
            return new ServiceResponse<T>
            {
                Succeeded = false,
                SessionId = ctx.SessionId,
                TransactionId = ctx.TransactionId,
                Errors = new List<ErrorDetail>
            {
                new ErrorDetail { Code = code, Message = message }
            }
            };
        }
    }

}

