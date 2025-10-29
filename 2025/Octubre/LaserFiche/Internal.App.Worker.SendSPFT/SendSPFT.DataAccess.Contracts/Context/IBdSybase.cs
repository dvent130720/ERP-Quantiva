namespace SendSPFT.DataAccess.Contracts.Context
{
    using System;
    using System.Collections.Generic;
    using System.Data.CData.Sybase;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface IBdSybase
    {
        public Task<SybaseConnection> CreateConnectionAsync();
        public Task<bool> IsHealthyAsync();
    }
}
