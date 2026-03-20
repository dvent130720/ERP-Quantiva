namespace SriFacturacion.Application.Abstracciones.Infraestructura;

public interface ICifrador
{
    string Cifrar(string textoPlano);
    string Descifrar(string textoCifrado);
}
