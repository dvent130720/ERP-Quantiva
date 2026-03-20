using SriFacturacion.Application.Abstracciones.Sri;
using SriFacturacion.Domain.Entidades;

namespace SriFacturacion.Infrastructure.Servicios.Documentos;

public sealed class GeneradorClaveAccesoEcuador : IGeneradorClaveAcceso
{
    public string Generar(Factura factura)
    {
        var fecha = DateTimeOffset.UtcNow.ToString("ddMMyyyy");
        var documento = new string(factura.NumeroDocumento.Where(char.IsDigit).ToArray()).PadLeft(15, '0');
        var baseClave = $"{fecha}01{factura.RucEmisor}{documento}123456781";
        var modulo11 = CalcularModulo11(baseClave);
        return $"{baseClave}{modulo11}";
    }

    private static int CalcularModulo11(string cadena)
    {
        var factores = new[] { 2, 3, 4, 5, 6, 7 };
        var indiceFactor = 0;
        var suma = 0;
        for (var i = cadena.Length - 1; i >= 0; i--)
        {
            suma += (cadena[i] - '0') * factores[indiceFactor];
            indiceFactor = (indiceFactor + 1) % factores.Length;
        }

        var residuo = 11 - (suma % 11);
        return residuo switch
        {
            11 => 0,
            10 => 1,
            _ => residuo
        };
    }
}
