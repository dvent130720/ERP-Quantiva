namespace SendSPFT.Business.Models.BD
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class FileRegBitacoraDto
    {
        public string Operacion { get; set; } = string.Empty;
        public string Proceso { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public string DescError { get; set; } = string.Empty;
        public DateTime FechaProceso { get; set; }
        public int? Error { get; set; }
        public string? MsgError { get; set; } = string.Empty;
    }
}
