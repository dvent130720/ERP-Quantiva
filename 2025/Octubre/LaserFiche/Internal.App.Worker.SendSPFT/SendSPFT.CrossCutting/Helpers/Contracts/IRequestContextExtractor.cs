namespace SendSPFT.CrossCutting.Helpers.Contracts
{


    using Microsoft.AspNetCore.Http;

    using SendSPFT.Bussiness.Models;

    public interface IRequestContextHeadersExtractor
    {
        RequestContextHeaders Extract(HttpContext httpContext);
    }
}

