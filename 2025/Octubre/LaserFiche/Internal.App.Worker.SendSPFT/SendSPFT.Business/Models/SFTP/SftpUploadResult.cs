namespace SendSPFT.Business.Models.SFTP
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public  class SftpUploadResult
    {
        public bool Success { get; set; }
        public string? RemoteFilePath { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
