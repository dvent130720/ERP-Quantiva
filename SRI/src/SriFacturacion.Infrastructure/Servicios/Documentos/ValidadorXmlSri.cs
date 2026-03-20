using System.Xml;
using System.Xml.Schema;
using Microsoft.Extensions.Logging;
using SriFacturacion.Application.Abstracciones.Documentos;

namespace SriFacturacion.Infrastructure.Servicios.Documentos;

public sealed class ValidadorXmlSri : IValidadorXmlSri
{
    private readonly ILogger<ValidadorXmlSri> _logger;

    public ValidadorXmlSri(ILogger<ValidadorXmlSri> logger)
    {
        _logger = logger;
    }

    public Task ValidarAsync(string xml, CancellationToken cancellationToken)
    {
        var esquemas = new XmlSchemaSet();
        using var schemaReader = XmlReader.Create(new StringReader(EsquemaFactura));
        esquemas.Add(string.Empty, schemaReader);

        var configuracion = new XmlReaderSettings
        {
            ValidationType = ValidationType.Schema,
            Schemas = esquemas,
            Async = true
        };

        var errores = new List<string>();
        configuracion.ValidationEventHandler += (_, args) => errores.Add(args.Message);

        using var stringReader = new StringReader(xml);
        using var xmlReader = XmlReader.Create(stringReader, configuracion);
        while (xmlReader.Read()) { }

        if (errores.Count > 0)
        {
            _logger.LogWarning("XML inválido: {Errores}", string.Join(" | ", errores));
            throw new InvalidOperationException($"XML inválido para SRI: {string.Join(" | ", errores)}");
        }

        return Task.CompletedTask;
    }

    private const string EsquemaFactura = """
    <xs:schema xmlns:xs='http://www.w3.org/2001/XMLSchema'>
      <xs:element name='factura'>
        <xs:complexType>
          <xs:sequence>
            <xs:element name='infoTributaria'/>
            <xs:element name='infoFactura'/>
            <xs:element name='detalles'/>
          </xs:sequence>
          <xs:attribute name='id' type='xs:string' use='required'/>
          <xs:attribute name='version' type='xs:string' use='required'/>
        </xs:complexType>
      </xs:element>
    </xs:schema>
    """;
}
