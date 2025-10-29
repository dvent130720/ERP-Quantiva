namespace SendSPFT.Business.Models.BD
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class TdAttachmentDto
    {
        public int TdId { get; set; }
        public string TdCedula { get; set; } = string.Empty;
        public string TdTipoIdent { get; set; } = string.Empty;
        public string TdIdOperation { get; set; } = string.Empty;
        public string TdModelo { get; set; } = string.Empty;
        public string TdApplicationId { get; set; } = string.Empty;
        public string TdDomainHost { get; set; } = string.Empty;
        public string TdReportId { get; set; } = string.Empty;
        public string TdAdjIdOperation { get; set; } = string.Empty;
        public string TdFormat { get; set; } = string.Empty;
        public string TdBase64 { get; set; } = string.Empty;
        public string TdOperacion { get; set; } = string.Empty;
        public string TdReportName { get; set; } = string.Empty;
        public DateTime? TdOperacionFechaRegistro { get; set; }
        public DateTime? TdOperacionFechaEnvio { get; set; }

    }
}
