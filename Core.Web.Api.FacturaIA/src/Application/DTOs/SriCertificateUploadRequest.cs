namespace Core.Web.Api.FacturaIA.Application.DTOs;

public class SriCertificateUploadRequest
{
    public string Password { get; set; } = string.Empty;
    public string Provider { get; set; } = "SRI";
}
