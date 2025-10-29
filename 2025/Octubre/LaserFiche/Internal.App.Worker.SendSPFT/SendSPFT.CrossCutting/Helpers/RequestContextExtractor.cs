namespace SendSPFT.CrossCutting.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using Davivienda.Framework.Lib.VoucherNumber;

    using Microsoft.AspNetCore.Http;

    using SendSPFT.Bussiness.Models;
    using SendSPFT.CrossCutting.Helpers.Contracts;

    public class RequestContextHeadersExtractor(IVoucherNumber voucherGenerator) : IRequestContextHeadersExtractor
    {
        public RequestContextHeaders Extract(HttpContext httpContext)
        {
            string GetHeader(string key) => httpContext.Request.Headers.TryGetValue(key, out var value) ? value.ToString() : string.Empty;

            return new RequestContextHeaders
            {
                SessionId = GetHeader("x-SessionId"),
                TransactionId = GetHeader("x-TransactionId"),
                ChannelId = GetHeader("x-ChannelId"),
                I18n = GetHeader("x-I18n"),
                ServiceId = GetHeader("x-ServiceId"),
                VoucherNumber = voucherGenerator.GenerateVoucherNumber()?.NumeroComprobante ?? "0"
            };
        }
    }
}

