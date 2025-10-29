namespace GetVirtualKey.Application.Contracts.DTOs
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using global:: Business.Models.RequestResponse;

  
    using SendSPFT.Bussiness.Models;

    public class RequestSnsDto
    {
        public RequestContextHeaders Header { get; set; }
        public GetVirtualKeyRequest Data { get; set; }
        
    }
}

