namespace SendSPFT.Business.Models.BD
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class GetDatesSyb12ResponseDto
    {
        public DateTime FechaProceso { get; set; }
        public DateTime? FechaDia { get; set; }
        public DateTime? FechaHora { get; set; }
        public int NumError { get; set; }
        public string MsgError { get; set; } = string.Empty;

    }
}
