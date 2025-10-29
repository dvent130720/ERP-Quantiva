namespace SendSPFT.CrossCutting.Helpers.Contracts
{
    using System;


    using Microsoft.Extensions.Logging;

    using SendSPFT.Bussiness.Models;

    public interface IRequestLogger
    {
        void LogRequest(object body, RequestContextHeaders context, ILogger logger);
        void LogSuccess(RequestContextHeaders context);
        void LogError(RequestContextHeaders context, Exception ex);
    }
}

