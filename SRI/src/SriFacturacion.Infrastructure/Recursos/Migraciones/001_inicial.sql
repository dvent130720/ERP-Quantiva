CREATE TABLE IF NOT EXISTS Certificados (
    Id UUID PRIMARY KEY,
    NombreArchivo TEXT NOT NULL,
    RutaArchivoSeguro TEXT NOT NULL,
    ClaveCifrada TEXT NOT NULL,
    Thumbprint TEXT NOT NULL,
    RucTitular VARCHAR(13) NOT NULL,
    FechaExpiracion TIMESTAMPTZ NOT NULL,
    Activo BOOLEAN NOT NULL,
    FechaCreacion TIMESTAMPTZ NOT NULL
);

CREATE TABLE IF NOT EXISTS Facturas (
    Id UUID PRIMARY KEY,
    NumeroDocumento VARCHAR(17) NOT NULL,
    RucEmisor VARCHAR(13) NOT NULL,
    RazonSocialEmisor TEXT NOT NULL,
    CorreoCliente TEXT NOT NULL,
    IdentificacionCliente VARCHAR(13) NOT NULL,
    NombreCliente TEXT NOT NULL,
    Moneda VARCHAR(3) NOT NULL,
    Subtotal NUMERIC(18,2) NOT NULL,
    Impuestos NUMERIC(18,2) NOT NULL,
    Total NUMERIC(18,2) NOT NULL,
    ClaveAcceso VARCHAR(60) NOT NULL,
    XmlGenerado TEXT NOT NULL,
    XmlFirmado TEXT NULL,
    XmlAutorizado TEXT NULL,
    Estado VARCHAR(50) NOT NULL,
    MensajeEstado TEXT NOT NULL,
    FechaCreacion TIMESTAMPTZ NOT NULL,
    FechaAutorizacion TIMESTAMPTZ NULL,
    CertificadoId UUID NOT NULL REFERENCES Certificados(Id)
);

CREATE TABLE IF NOT EXISTS FacturaItems (
    Id UUID PRIMARY KEY,
    FacturaId UUID NOT NULL REFERENCES Facturas(Id) ON DELETE CASCADE,
    CodigoPrincipal TEXT NOT NULL,
    Descripcion TEXT NOT NULL,
    Cantidad NUMERIC(18,2) NOT NULL,
    PrecioUnitario NUMERIC(18,2) NOT NULL,
    PorcentajeDescuento NUMERIC(18,2) NOT NULL,
    BaseImponible NUMERIC(18,2) NOT NULL,
    CodigoImpuesto VARCHAR(10) NOT NULL,
    TarifaImpuesto NUMERIC(18,2) NOT NULL,
    ValorImpuesto NUMERIC(18,2) NOT NULL
);

CREATE TABLE IF NOT EXISTS Catalogos (
    Id UUID PRIMARY KEY,
    Tipo VARCHAR(100) NOT NULL,
    Codigo VARCHAR(50) NOT NULL,
    Valor VARCHAR(100) NOT NULL,
    Activo BOOLEAN NOT NULL
);

CREATE TABLE IF NOT EXISTS Logs (
    Id UUID PRIMARY KEY,
    CorrelationId VARCHAR(100) NOT NULL,
    Nivel VARCHAR(20) NOT NULL,
    Mensaje TEXT NOT NULL,
    Datos JSONB NULL,
    Fecha TIMESTAMPTZ NOT NULL
);

CREATE UNIQUE INDEX IF NOT EXISTS ux_catalogos_tipo_codigo ON Catalogos(Tipo, Codigo);
CREATE INDEX IF NOT EXISTS ix_facturas_estado ON Facturas(Estado);
CREATE INDEX IF NOT EXISTS ix_facturas_clave_acceso ON Facturas(ClaveAcceso);

INSERT INTO Catalogos (Id, Tipo, Codigo, Valor, Activo)
VALUES
    ('00000000-0000-0000-0000-000000000101', 'IVA', '2', '15', TRUE),
    ('00000000-0000-0000-0000-000000000102', 'IVA', '0', '0', TRUE)
ON CONFLICT (Tipo, Codigo) DO NOTHING;
