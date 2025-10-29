namespace SendSPFT.DataAccess.Contracts.Exec.Func
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using Npgsql;

    using SendSPFT.Business.Models.BD;

    public interface ISelectRegistryBD
    {
        Task<List<TdAttachmentDto>> SelectRecordAsync();
        TdAttachmentDto MapReaderToDto(NpgsqlDataReader reader);
    }
}
