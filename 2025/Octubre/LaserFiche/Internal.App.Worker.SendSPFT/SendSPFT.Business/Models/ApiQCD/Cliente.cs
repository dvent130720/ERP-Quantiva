using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Customer.Web.Api.ConsultaDatosClientes.Business.Models
{
    public class Cliente
    {
        public Cliente(String pmMensaje)
        {
            this.MensajeErrorOut = pmMensaje;
        }

        public Cliente()
        {

        }

        public String? TipoCliente { get; set; }
        public String? Identificacion { get; set; }
        public String? TipoIdentificacion { get; set; }
        public String? FechaVencCedula { get; set; }
        public String? PrimerNombre { get; set; }
        public String? SegundoNombre { get; set; }
        public String? PrimerApellido { get; set; }
        public String? SegundoApellido { get; set; }
        public String? Genero { get; set; }
        public String? EstadoCivil { get; set; }
        public String? FechaNacimiento { get; set; }
        public String? PaisNacimiento { get; set; }
        public String? Nacionalidad { get; set; }
        public String? OtraNacionalidad { get; set; }
        public String? Profesion { get; set; }
        public String? FuncionesPublicas { get; set; }
        public String? ManejoFondosTerceros { get; set; }
        public Decimal TotalIngresos { get; set; }
        public Decimal TotalEgresos { get; set; }
        public Decimal TotalActivos { get; set; }
        public Decimal TotalPasivos { get; set; }
        public int TransaccionesMes { get; set; }
        public String? OperacionesInternacionales { get; set; }
        public Decimal OperIntMontoEstimadoMensual { get; set; }
        public int OperIntTipoOperaciones { get; set; }
        public String? OperIntPaisPrincipalDesOri { get; set; }
        public String? OperIntBancoPrincipal { get; set; }
        public String? OperIntNombreRemBen { get; set; }
        public String? OperIntCuentaPrincipal { get; set; }
        public String? ResidenciaLegalEEUU { get; set; }
        public String? CiudadaniaNacionalidadEEUU { get; set; }
        public String? PasaporteEEUU { get; set; }
        public String? EstanciaUltimoAnioUSA { get; set; }
        public String? PromedioUltimoAnioUSA { get; set; }
        public String? MotivoEstadiaUSA { get; set; }
        public String? OtroMotivoEstadiaUSA { get; set; }

        public String? ObligacionesTributariasUSA { get; set; }
        public String? NumeroGIIN { get; set; }
        public String? SucursalFilialSubsidiaria { get; set; }
        public String? PaisCasaMatriz { get; set; }
        public String? NumeroEINoTIN { get; set; }
        public String? EmpTibutacionFiscalEEUU { get; set; }
        public String? EmpBeneFin { get; set; }
        public String? AccionistaParticipacion { get; set; }
        public String? SocioEmpDomFisNoEEUUCR { get; set; }
        public String? PEP { get; set; }
        public String? CodAreaCel { get; set; }
        public String? TelCelular { get; set; }
        public String? CodAreaDom { get; set; }
        public String? TelDom { get; set; }
        public String? CodAreaTra { get; set; }
        public String? TelTrabajo { get; set; }
        public String? URLPaginaWeb { get; set; }
        public String? DireccionCorreo { get; set; }
        public String? DescribaMotivoRelacion { get; set; }
        public String? ActividadRealiza { get; set; }
        public String? ManejaFondosPoliticos { get; set; }
        public String? NotificacionSMS { get; set; }
        public int TiempoResidirPais { get; set; }
        public String? Ente { get; set; }
        public String? UsuarioVendedor { get; set; }
        public String? NombreVendedor { get; set; }
        public String? Puesto { get; set; }
        public String? Sucursal { get; set; }
        public List<Direccion> ?Direcciones { get; set; }
        public List<Actividad>? Actividades { get; set; }
        public List<Ingresos>? lstIngresos { get; set; }
        public List<Socio>? Socios { get; set; }
        public List<ClienteAC>? ClientesAC { get; set; }
        public List<InfoFiscal>? lstInfoFiscal { get; set; }
        public String? MensajeErrorOut { get; set; }

    }
}
