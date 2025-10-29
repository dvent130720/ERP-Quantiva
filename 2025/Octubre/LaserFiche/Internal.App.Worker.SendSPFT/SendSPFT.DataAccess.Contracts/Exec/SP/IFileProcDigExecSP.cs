namespace SendSPFT.DataAccess.Contracts.Exec.SP
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using SendSPFT.Business.Models.BD;

    public interface IFileProcDigExecSP
    {
        Task<bool> ExecuteFileProcDigAsync(FileProcDigDto dto);
    }
}
