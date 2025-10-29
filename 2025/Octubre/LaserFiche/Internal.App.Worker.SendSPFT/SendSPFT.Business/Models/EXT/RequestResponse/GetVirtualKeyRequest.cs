namespace Business.Models.RequestResponse
{


    public class GetVirtualKeyRequest
    {
        public string TipoIdentificacion { get; set; } = string.Empty;
        public string NumeroIdentificacion { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
        public string? Impresora { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public string? ClaveDefinitiva { get; set; }

    }
}
