namespace SendSPFT.CrossCutting.Subservices.ErrorSftp
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface IErrorFileService
    {
        Task UploadErrorAsync(Exception exception, string context, string cedula);
    }
}
