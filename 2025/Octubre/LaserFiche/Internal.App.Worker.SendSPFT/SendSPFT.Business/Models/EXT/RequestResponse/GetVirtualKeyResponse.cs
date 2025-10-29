namespace Business.Models.RequestResponse
{

    public class GetVirtualKeyResponse
    {

        public string TipoIdentificacionP { get; set; } = string.Empty;
        public string NumeroIdentificacionP { get; set; } = string.Empty;
        public string TipoIdentificacionC { get; set; } = string.Empty;
        public string NumeroIdentificacionC { get; set; } = string.Empty;
        public string  EstadoSolicitud { get; set; } = string.Empty;


    }
}
