namespace Business.Models.RequestResponse
{
   
    public class PasswordChangeRequest
    {


        public string TipoIdentificacionP { get; set; } = string.Empty;
        public string NumeroIdentificacionP { get; set; } = string.Empty;
        public string TipoIdentificacionC { get; set; } = string.Empty;
        public string NumeroIdentificacionC { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
        public string Impresora { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string? Nombre2 { get; set; }
        public string Apellido { get; set; } = string.Empty;
        public string? Apellido2 { get; set; }
        public string? ClaveDefinitiva { get; set; }

    }
}
