namespace SendSPFT.Business.Models.BD
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class FileProcDigDto
    {
        public string Operacion { get; set; } = "I";
        public int CodigoSistema { get; set; }
        public string IdSolicitud { get; set; } = string.Empty;
        public string User { get; set; } = string.Empty;
        public DateTime FechaProceso { get; set; }
        public DateTime FechaHora { get; set; }
        public DateTime FechaEjecucion { get; set; }
        public string IdDoc { get; set; } = string.Empty;
        public string IdDocDef { get; set; } = string.Empty;
        public string NombreCompleto { get; set; } = string.Empty;
        public string Apellido1 { get; set; } = string.Empty;
        public string Apellido2 { get; set; } = string.Empty;
        public string RazonSocial { get; set; } = string.Empty;
        public string TipoIdent { get; set; } = string.Empty;
        public string NumeroCedula { get; set; } = string.Empty;
        public string TipoPersona { get; set; } = string.Empty;
        public string TipoProducto { get; set; } = string.Empty;
        public string NumeroOperacion { get; set; } = string.Empty;
        public string AnioPolitica { get; set; } = string.Empty;
        public int NumeroEnte { get; set; } 
        public string TipoDocumento { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public DateTime? FechaCreditoMovil { get; set; }
    }

}
