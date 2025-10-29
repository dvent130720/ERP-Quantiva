namespace SendSPFT.CrossCutting.Subservices
{
    using SendSPFT.Business.Models.BD;

    public interface IProcessRecord
    {
        Task ProcessRecordAsync(TdAttachmentDto record,  GetDatesSyb12ResponseDto dates);
    }
}