using Microsoft.AspNetCore.Mvc;
using SriFacturacion.Application.Abstracciones.Facturacion;
using SriFacturacion.Application.DTOs;

namespace SriFacturacion.Api.Controllers;

[ApiController]
[Route("sri/certificados")]
public sealed class CertificadosController : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(SubirCertificadoResponse), StatusCodes.Status201Created)]
    public async Task<IActionResult> SubirCertificado(
        [FromServices] IServicioCertificados servicioCertificados,
        [FromForm] IFormFile archivo,
        [FromForm] string clave,
        [FromForm] string rucTitular,
        CancellationToken cancellationToken)
    {
        await using var stream = archivo.OpenReadStream();
        using var memoria = new MemoryStream();
        await stream.CopyToAsync(memoria, cancellationToken);

        var request = new SubirCertificadoRequest
        {
            NombreArchivo = archivo.FileName,
            Archivo = memoria.ToArray(),
            Clave = clave,
            RucTitular = rucTitular
        };

        var respuesta = await servicioCertificados.SubirAsync(request, cancellationToken);
        return Created($"/sri/certificados/{respuesta.CertificadoId}", respuesta);
    }
}
