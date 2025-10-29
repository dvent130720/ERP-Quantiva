namespace SendSPFT.Application.Contracts.Clients
{


    using GetVirtualKey.Application.Contracts.DTOs;

    using SendSPFT.Business.Models.SFTP;

    public interface ISftpFileUploader
    {
        Task<SftpUploadResult> UploadFileAsync(string format, string base64Content, string reportName);
        Task<SftpUploadResult> UploadFileInternalAsync(string format, string base64Content, string reportName);

        Task<SftpUploadResult> UploadErrorFileAsync(string format, string base64Content, string reportName);


    }
}
