using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SendSPFT.Business.Models.BD;

namespace SendSPFT.DataAccess.Contracts.Exec.SP
{
    public interface ISelectRecordByDate
    {
        Task<IEnumerable<string>> SelectPendingAsync(DateTime processDate, CancellationToken cancellationToken);
    }
}
