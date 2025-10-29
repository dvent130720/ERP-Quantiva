namespace SendSPFT.Bussiness.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class RequestContextHeaders
    {
        public string SessionId { get; set; } = string.Empty;
        public string TransactionId { get; set; } = string.Empty;
        public string ChannelId { get; set; } = string.Empty;
        public string I18n { get; set; } = string.Empty;
        public string ServiceId { get; set; } = string.Empty;
        public string VoucherNumber { get; set; } = string.Empty;
    }
}

