using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

using Davivienda.Framework.Lib.BitacoraLog.BitacoraLog;
using Davivienda.Framework.Lib.VoucherNumber;
using Davivienda.Framework.Services.CanonicalSignature;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

using Moq;

using SendSPFT.Business.Models.BD;
using SendSPFT.Bussiness.Models;
using SendSPFT.CrossCutting.Config;
using SendSPFT.CrossCutting.Helpers;
using SendSPFT.CrossCutting.Models.HealthCheckModels;

using Swashbuckle.AspNetCore.SwaggerGen;

using Xunit;

namespace SendSPFT.Test
{
    public class CrossCuttingHelpersTests
    {
        #region RequestContextHeadersExtractor Tests

        [Fact]
        public void ExtractShouldReturnHeadersFromHttpContext()
        {
            var mockVoucher = new Mock<IVoucherNumber>();
            mockVoucher.Setup(v => v.GenerateVoucherNumber())
                .Returns(new VoucherNumberDto { NumeroComprobante = "12345" });

            var extractor = new RequestContextHeadersExtractor(mockVoucher.Object);

            var context = new DefaultHttpContext();
            context.Request.Headers["x-SessionId"] = "sess-1";
            context.Request.Headers["x-TransactionId"] = "trans-1";
            context.Request.Headers["x-ChannelId"] = "chan-1";
            context.Request.Headers["x-I18n"] = "es-CO";
            context.Request.Headers["x-ServiceId"] = "service-1";

            var result = extractor.Extract(context);

            Assert.Equal("sess-1", result.SessionId);
            Assert.Equal("trans-1", result.TransactionId);
            Assert.Equal("chan-1", result.ChannelId);
            Assert.Equal("es-CO", result.I18n);
            Assert.Equal("service-1", result.ServiceId);
            Assert.Equal("12345", result.VoucherNumber);
        }

        #endregion

        #region RequestLogger Tests

        [Fact]
        public void LogRequestShouldCallRegistrar()
        {
            var mockBitacora = new Mock<IBitacoraLog>();
            var logger = new Mock<ILogger>();
            var requestLogger = new RequestLogger(mockBitacora.Object);

            var context = new RequestContextHeaders
            {
                TransactionId = "T1",
                SessionId = "S1",
                ChannelId = "C1",
                VoucherNumber = "V1"
            };

            requestLogger.LogRequest(new { Data = "test" }, context, logger.Object);

            mockBitacora.Verify(b => b.Registrar(It.IsAny<BitacoraLogDto>()), Times.Once);
        }

        [Fact]
        public void LogErrorShouldIncludeExceptionMessage()
        {
            var mockBitacora = new Mock<IBitacoraLog>();
            var requestLogger = new RequestLogger(mockBitacora.Object);

            var context = new RequestContextHeaders
            {
                TransactionId = "T1",
                SessionId = "S1",
                ChannelId = "C1",
                VoucherNumber = "V1"
            };
            var ex = new InvalidOperationException("Test error");

            requestLogger.LogError(context, ex);

            mockBitacora.Verify(b => b.Registrar(It.Is<BitacoraLogDto>(log => log.DescError == "Test error")), Times.Once);
        }

        #endregion

        #region ServiceResponseFactory Tests

        [Fact]
        public void SuccessShouldReturnSucceededResponse()
        {
            var factory = new ServiceResponseFactory();
            var ctx = new RequestContextHeaders { SessionId = "S1", TransactionId = "T1" };
            var data = new { Name = "Diego" };

            var response = factory.Success(data, ctx);

            Assert.True(response.Succeeded);
            Assert.Equal("S1", response.SessionId);
            Assert.Equal("T1", response.TransactionId);
            Assert.Single(response.Errors ?? new List<ErrorDetail>());
            Assert.Equal("0", (response.Errors ?? new List<ErrorDetail>())[0].Code);
        }

        [Fact]
        public void FailShouldReturnFailedResponse()
        {
            var factory = new ServiceResponseFactory();
            var ctx = new RequestContextHeaders { SessionId = "S1", TransactionId = "T1" };

            var response = factory.Fail<object>("999", "Error ocurrido", ctx);

            Assert.False(response.Succeeded);
            Assert.Equal("S1", response.SessionId);
            Assert.Equal("T1", response.TransactionId);
            Assert.Single(response.Errors ?? new List<ErrorDetail>());
            Assert.Equal("999", (response.Errors ?? new List<ErrorDetail>())[0].Code);
            Assert.Equal("Error ocurrido", (response.Errors ?? new List<ErrorDetail>())[0].Message);
        }

        #endregion

        #region SwaggerHeaderFilter Tests

        [Fact]
        public void ApplyShouldAddHeaders()
        {
            var filter = new SwaggerHeaderFilter();
            var operation = new OpenApiOperation();
            var context = new OperationFilterContext(null, null, null, null);

            filter.Apply(operation, context);

            Assert.Contains(operation.Parameters, p => p.Name == "x-session-id");
            Assert.Contains(operation.Parameters, p => p.Name == "x-transaction-id");
            Assert.Contains(operation.Parameters, p => p.Name == "x-channel");
            Assert.Contains(operation.Parameters, p => p.Name == "x-i18n");
        }

        #endregion

        #region FileProcDigDto Tests

        [Fact]
        public void PropertiesShouldSetAndGetCorrectly()
        {
            var dto = new FileProcDigDto();

            dto.Operacion = "U";
            dto.CodigoSistema = 42;
            dto.IdSolicitud = "REQ123";
            dto.User = "user1";
            dto.FechaProceso = new DateTime(2025, 10, 1, 0, 0, 0, DateTimeKind.Utc);
            dto.FechaHora = new DateTime(2025, 10, 1, 12, 0, 0, DateTimeKind.Utc);
            dto.FechaEjecucion = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);
            dto.IdDoc = "DOC001";
            dto.IdDocDef = "DEF001";
            dto.NombreCompleto = "Juan Pérez";
            dto.Apellido1 = "Pérez";
            dto.Apellido2 = "Gómez";
            dto.RazonSocial = "Empresa S.A.";
            dto.TipoIdent = "CC";
            dto.NumeroCedula = "1234567890";
            dto.TipoPersona = "N";
            dto.TipoProducto = "PRD1";
            dto.NumeroOperacion = "OP123";
            dto.AnioPolitica = "2025";
            dto.NumeroEnte = 7;
            dto.TipoDocumento = "TI";
            dto.Estado = "Activo";
            dto.FechaCreditoMovil = new DateTime(2025, 10, 1, 0, 0, 0, DateTimeKind.Utc);

            Assert.Equal("U", dto.Operacion);
            Assert.Equal(42, dto.CodigoSistema);
            Assert.Equal("REQ123", dto.IdSolicitud);
            Assert.Equal("user1", dto.User);
            Assert.Equal(new DateTime(2025, 10, 1, 0, 0, 0, DateTimeKind.Utc), dto.FechaProceso);
            Assert.Equal(new DateTime(2025, 10, 1, 12, 0, 0, DateTimeKind.Utc), dto.FechaHora);
            Assert.Equal("DOC001", dto.IdDoc);
            Assert.Equal("DEF001", dto.IdDocDef);
            Assert.Equal("Juan Pérez", dto.NombreCompleto);
            Assert.Equal("Pérez", dto.Apellido1);
            Assert.Equal("Gómez", dto.Apellido2);
            Assert.Equal("Empresa S.A.", dto.RazonSocial);
            Assert.Equal("CC", dto.TipoIdent);
            Assert.Equal("1234567890", dto.NumeroCedula);
            Assert.Equal("N", dto.TipoPersona);
            Assert.Equal("PRD1", dto.TipoProducto);
            Assert.Equal("OP123", dto.NumeroOperacion);
            Assert.Equal("2025", dto.AnioPolitica);
            Assert.Equal(7, dto.NumeroEnte);
            Assert.Equal("TI", dto.TipoDocumento);
            Assert.Equal("Activo", dto.Estado);
            Assert.Equal(new DateTime(2025, 10, 1, 0, 0, 0, DateTimeKind.Utc), dto.FechaCreditoMovil);
        }

        #endregion
    }
}
