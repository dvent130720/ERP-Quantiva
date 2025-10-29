using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Customer.Web.Api.ConsultaDatosClientes.Business.Models
{
    public class Direccion
    {
        public String? TipoDireccion { get; set; }
        public String? Pais { get; set; }
        public String? Provincia { get; set; }
        public String? Canton { get; set; }
        public String? Distrito { get; set; }
        public String? DireccionDetallada { get; set; }

    }
}
