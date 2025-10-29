namespace SendSPFT.DataAccess.Exec.SP
{
    using System;
    using System.Data;
    using System.Threading.Tasks;

    using Microsoft.Data.SqlClient;
    using Microsoft.Extensions.Configuration;

    using SendSPFT.Business.Models.BD;
    using SendSPFT.DataAccess.Contracts.Exec.SP;

    public class FileProcDigExecSP : IFileProcDigExecSP
    {
        private readonly IConfiguration _configuration;

        public FileProcDigExecSP(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<bool> ExecuteFileProcDigAsync(FileProcDigDto dto)
        {
            var connectionString = _configuration.GetConnectionString("EDIntefaz");

            await using var connection = new SqlConnection(connectionString);
            await using var command = new SqlCommand("dbo.sp_mb_file_proc_dig", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

         
            command.Parameters.AddWithValue("@i_operacion", dto.Operacion);
            command.Parameters.AddWithValue("@i_codigo_sistema", dto.CodigoSistema);
            command.Parameters.AddWithValue("@i_id_solicitud", dto.IdSolicitud);
            command.Parameters.AddWithValue("@i_user", dto.User);
            command.Parameters.AddWithValue("@i_numero_cedula", dto.NumeroCedula);

           
            command.Parameters.AddWithValue("@i_fecha_proceso", dto.FechaProceso.ToSqlDateTimeOrNull());
            command.Parameters.AddWithValue("@i_fecha_hora", dto.FechaHora.ToSqlDateTimeOrNull());
            command.Parameters.AddWithValue("@i_fecha_ejecucion", dto.FechaEjecucion.ToSqlDateTimeOrNull());
            command.Parameters.AddWithValue("@i_fecha_credito_movil", dto.FechaCreditoMovil.ToSqlDateTimeOrNull());

          
            command.Parameters.AddWithValue("@i_id_doc", dto.IdDoc ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@i_id_doc_def", dto.IdDocDef ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@i_nombre_completo", dto.NombreCompleto ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@i_apellido1", dto.Apellido1 ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@i_apellido2", dto.Apellido2 ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@i_razon_social", dto.RazonSocial ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@i_tipo_ident", dto.TipoIdent ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@i_tipo_persona", dto.TipoPersona ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@i_tipo_producto", dto.TipoProducto ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@i_numero_operacion", dto.NumeroOperacion ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@i_anio_politica", dto.AnioPolitica ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@i_tipo_documento", dto.TipoDocumento ?? (object)DBNull.Value);

         
            command.Parameters.AddWithValue("@i_numero_ente", dto.NumeroEnte);

         
            command.Parameters.AddWithValue("@i_estado", string.IsNullOrWhiteSpace(dto.Estado) ? 'P' : dto.Estado[0]);

            await connection.OpenAsync();
            var result = await command.ExecuteNonQueryAsync();
            return result > 0;
        }
    }

    public static class SqlParameterExtensions
    {
        private static readonly DateTime MinSqlDate = new DateTime(1753, 1, 1, 0, 0, 0, DateTimeKind.Utc);


        public static object ToSqlDateTimeOrNull(this DateTime date)
        {
            return date < MinSqlDate ? DBNull.Value : date;
        }

        public static object ToSqlDateTimeOrNull(this DateTime? date)
        {
            return date.HasValue && date.Value >= MinSqlDate ? date.Value : DBNull.Value;
        }
    }
}
