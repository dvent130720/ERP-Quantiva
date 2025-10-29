using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Customer.Web.Api.ConsultaDatosClientes.Business.Models
{
    public class Ingresos
    {
        public String? TipoIngresoMensual { get; set; }
        public Decimal DetalleIngresoMensual { get; set; }
        public Decimal OtrosIngresosMensual { get; set; }
        public String? DetalleOtrosIngresos { get; set; }
        public String? CodMoneda { get; set; }
    }
}
