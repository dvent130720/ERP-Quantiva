using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using SriFacturacion.Application.Abstracciones.Infraestructura;
using SriFacturacion.Infrastructure.Opciones;

namespace SriFacturacion.Infrastructure.Cifrado;

public sealed class AesCifrador : ICifrador
{
    private readonly byte[] _llave;

    public AesCifrador(IOptions<SeguridadOptions> options)
    {
        _llave = Convert.FromBase64String(options.Value.LlaveAesBase64);
        if (_llave.Length != 32)
        {
            throw new InvalidOperationException("La llave AES debe tener 32 bytes (AES-256).");
        }
    }

    public string Cifrar(string textoPlano)
    {
        using var aes = Aes.Create();
        aes.Key = _llave;
        aes.GenerateIV();
        using var cifrador = aes.CreateEncryptor();
        var bytesPlano = Encoding.UTF8.GetBytes(textoPlano);
        var bytesCifrados = cifrador.TransformFinalBlock(bytesPlano, 0, bytesPlano.Length);
        return $"{Convert.ToBase64String(aes.IV)}:{Convert.ToBase64String(bytesCifrados)}";
    }

    public string Descifrar(string textoCifrado)
    {
        var partes = textoCifrado.Split(':', 2, StringSplitOptions.TrimEntries);
        if (partes.Length != 2)
        {
            throw new InvalidOperationException("Formato cifrado inválido.");
        }

        using var aes = Aes.Create();
        aes.Key = _llave;
        aes.IV = Convert.FromBase64String(partes[0]);
        using var descifrador = aes.CreateDecryptor();
        var bytes = Convert.FromBase64String(partes[1]);
        var plano = descifrador.TransformFinalBlock(bytes, 0, bytes.Length);
        return Encoding.UTF8.GetString(plano);
    }
}
