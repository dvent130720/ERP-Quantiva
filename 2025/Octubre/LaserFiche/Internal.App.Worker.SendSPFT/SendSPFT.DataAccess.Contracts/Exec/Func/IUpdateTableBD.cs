namespace SendSPFT.DataAccess.Contracts.Exec.Func
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using SendSPFT.Business.Models.BD;

    public interface IUpdateTableBD
    {
        Task UpdateTableAsync(TdAttachmentDto record);
    }
}
