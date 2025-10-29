using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Npgsql;

namespace SendSPFT.DataAccess.Contracts.Context
{
    public interface ISendSftpContext
    {
        public Task<NpgsqlConnection> CreateConnectionAsync();
        public Task<bool> IsHealthyAsync();
    }
}

