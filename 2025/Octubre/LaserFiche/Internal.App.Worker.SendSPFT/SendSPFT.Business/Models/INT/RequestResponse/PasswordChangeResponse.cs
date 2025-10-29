namespace Business.Models.RequestResponse
{

    public class PasswordChangeResponse
    {
        public string Login { get; set; } = string.Empty;
        public string ValNombreUsuario { get; set; } = string.Empty;
        public string CodigoRespuesta { get; set; } = string.Empty;
        public string DescripcionRespuesta { get; set; } = string.Empty;
        public string CorrelationId { get; set; } = string.Empty;
    }
}
