using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using SendSPFT.DataAccess.Contracts.Exec.SP;

namespace SendSPFT.DataAccess.Exec.SP
{
    public class SelectRecordByDateExecSP : ISelectRecordByDate
    {
        private readonly IConfiguration _configuration;

        public SelectRecordByDateExecSP(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<IEnumerable<string>> SelectPendingAsync(DateTime processDate, CancellationToken cancellationToken)
        {
            var records = new List<string>();
            var connectionString = _configuration.GetConnectionString("EDIntefaz");

            await using var connection = new SqlConnection(connectionString);
            await using var command = new SqlCommand("dbo.sp_mb_consulta_file_proc_dig", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@i_fecha_proceso", processDate.ToSqlDateTimeOrNull());

            await connection.OpenAsync(cancellationToken);

            await using var reader = await command.ExecuteReaderAsync(cancellationToken);
            while (await reader.ReadAsync(cancellationToken))
            {
                var idDocDef = reader["fp_id_doc_def"]?.ToString();
                records.Add(idDocDef);
            }

            return records;
        }
    }
}
