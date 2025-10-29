using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Customer.Web.Api.ConsultaDatosClientes.Business.Models
{
    public class Socio
    {
        public String? PrimerNombreSoc { get; set; }
        public String? SegundoNombreSoc { get; set; }
        public String? PrimerApellidoSoc { get; set; }
        public String? SegundoApellidoSoc { get; set; }
        public String? TipoIdentificacionSoc { get; set; }
        public String? NumeroIdentificacion { get; set; }
        public String? Nacionalidad { get; set; }
        public Decimal Participacion { get; set; }
    }
}
