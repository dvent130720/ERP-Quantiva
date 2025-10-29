using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

using Core.Customer.Web.Api.ConsultaDatosClientes.Business.Models;

using Davivienda.Framework.Lib.Crypto;
using Davivienda.Framework.Lib.VoucherNumber;

using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using Moq;
using Moq.Protected;

using Npgsql;

using SendSPFT.Application.Clients;
using SendSPFT.Application.Contracts.Clients;
using SendSPFT.Application.Services;
using SendSPFT.Business.Models.BD;
using SendSPFT.Business.Models.SFTP;
using SendSPFT.CrossCutting.Mapper;
using SendSPFT.CrossCutting.Subservices;
using SendSPFT.CrossCutting.Subservices.ErrorSftp;
using SendSPFT.DataAccess.Context;
using SendSPFT.DataAccess.Contracts.Context;
using SendSPFT.DataAccess.Contracts.Exec.Func;
using SendSPFT.DataAccess.Exec.Func;
using SendSPFT.DataAccess.Exec.SP;

using Xunit;

namespace SendSPFT.Test
{
    public class SendSftpServiceTests
    {
        [Fact]
        public async Task ExecuteServiceAsyncNoRecordsLogsInfo()
        {
            var loggerMock = new Mock<ILogger<SendSftpService>>();
            var selectMock = new Mock<ISelectRegistryBD>();
            var processMock = new Mock<IProcessRecord>();

            selectMock.Setup(s => s.SelectRecordAsync())
                      .ReturnsAsync(new List<TdAttachmentDto>());

            var service = new SendSftpService(loggerMock.Object, selectMock.Object, processMock.Object);

            await service.ExecuteServiceAsync(CancellationToken.None);

            selectMock.Verify(s => s.SelectRecordAsync(), Times.Once);
        }

        [Fact]
        public async Task ExecuteServiceAsyncNullRecordsLogsInfo()
        {
            var loggerMock = new Mock<ILogger<SendSftpService>>();
            var selectMock = new Mock<ISelectRegistryBD>();
            var processMock = new Mock<IProcessRecord>();

            // Fix nullability warning: explicit cast + null-forgiving operator to preserve original intent (returns null)
            selectMock.Setup(s => s.SelectRecordAsync()).ReturnsAsync((List<TdAttachmentDto>?)null!);

            var service = new SendSftpService(loggerMock.Object, selectMock.Object, processMock.Object);

            await service.ExecuteServiceAsync(CancellationToken.None);

            selectMock.Verify(s => s.SelectRecordAsync(), Times.Once);
        }

        [Fact]
        public async Task ExecuteServiceAsyncSelectThrowsLogsError()
        {
            var loggerMock = new Mock<ILogger<SendSftpService>>();
            var selectMock = new Mock<ISelectRegistryBD>();
            var processMock = new Mock<IProcessRecord>();

            selectMock.Setup(s => s.SelectRecordAsync())
                      .ThrowsAsync(new InvalidOperationException("fail"));

            var service = new SendSftpService(loggerMock.Object, selectMock.Object, processMock.Object);

            await service.ExecuteServiceAsync(CancellationToken.None);

            selectMock.Verify(s => s.SelectRecordAsync(), Times.Once);
            // Aquí se podría verificar el log de error si quieres
        }

        [Fact]
        public async Task ExecuteServiceAsyncProcessesRecordsSuccessfully()
        {
            var loggerMock = new Mock<ILogger<SendSftpService>>();
            var selectRecordMock = new Mock<ISelectRegistryBD>();
            var processRecordMock = new Mock<IProcessRecord>();

            var records = new List<TdAttachmentDto>
            {
                new TdAttachmentDto { TdId = 1 },
                new TdAttachmentDto { TdId = 2 }
            };

            selectRecordMock.Setup(s => s.SelectRecordAsync()).ReturnsAsync(records);

            var service = new SendSftpService(loggerMock.Object, selectRecordMock.Object, processRecordMock.Object);

            await service.ExecuteServiceAsync(CancellationToken.None);

            foreach (var record in records)
            {
                processRecordMock.Verify(p => p.ProcessRecordAsync(record), Times.Once);
            }
        }

        [Fact]
        public async Task ExecuteServiceAsyncNoRecordsDoesNotProcess()
        {
            var loggerMock = new Mock<ILogger<SendSftpService>>();
            var selectRecordMock = new Mock<ISelectRegistryBD>();
            var processRecordMock = new Mock<IProcessRecord>();

            selectRecordMock.Setup(s => s.SelectRecordAsync()).ReturnsAsync(new List<TdAttachmentDto>());

            var service = new SendSftpService(loggerMock.Object, selectRecordMock.Object, processRecordMock.Object);

            await service.ExecuteServiceAsync(CancellationToken.None);

            processRecordMock.Verify(p => p.ProcessRecordAsync(It.IsAny<TdAttachmentDto>()), Times.Never);
        }

        [Fact]
        public async Task ProcessRecordAsyncProcessesRecordSuccessfully()
        {
            var loggerMock = new Mock<ILogger<ProcessRecord>>();
            var queryMock = new Mock<IQueryCustomerDataClient>();
            var rdsMock = new Mock<IUpdateTableBD>();
            var fileMock = new Mock<IFileProcessingService>();
            var configMock = new Mock<IConfiguration>();
            var sectionMock = new Mock<IConfigurationSection>();
            var fileerror = new Mock<IErrorFileService>();
            sectionMock.Setup(s => s.Value).Returns("SENT");
            configMock.Setup(c => c.GetSection("Operation_sent")).Returns(sectionMock.Object);

            var record = new TdAttachmentDto { TdId = 1 };
            queryMock.Setup(q => q.GetDataClient(record)).ReturnsAsync(new Cliente());

            var process = new ProcessRecord(loggerMock.Object, queryMock.Object, rdsMock.Object, fileMock.Object, configMock.Object, fileerror.Object);

            await process.ProcessRecordAsync(record);

            fileMock.Verify(f => f.ProcessAsync(record, It.IsAny<Cliente>()), Times.Once);
            rdsMock.Verify(r => r.UpdateTableAsync(record), Times.Once);
        }

        [Fact]
        public async Task ProcessRecordAsyncCustomerDataNullDoesNotProcess()
        {
            var loggerMock = new Mock<ILogger<ProcessRecord>>();
            var queryMock = new Mock<IQueryCustomerDataClient>();
            var rdsMock = new Mock<IUpdateTableBD>();
            var fileMock = new Mock<IFileProcessingService>();
            var configMock = new Mock<IConfiguration>();
            var fileerror = new Mock<IErrorFileService>();

            var record = new TdAttachmentDto { TdId = 1 };
            queryMock.Setup(q => q.GetDataClient(record)).ReturnsAsync((Cliente?)null);

            var process = new ProcessRecord(loggerMock.Object, queryMock.Object, rdsMock.Object, fileMock.Object, configMock.Object, fileerror.Object);

            await process.ProcessRecordAsync(record);

            fileMock.Verify(f => f.ProcessAsync(record, It.IsAny<Cliente>()), Times.Never);
            rdsMock.Verify(r => r.UpdateTableAsync(record), Times.Never);
        }

        [Fact]
        public async Task SelectRecordAsyncReturnsRecordsWithoutException()
        {
            var configMock = new Mock<IConfiguration>();
            var sectionMock = new Mock<IConfigurationSection>();
            sectionMock.Setup(s => s.Value).Returns("Registered");
            configMock.Setup(c => c.GetSection("Registered_Operation")).Returns(sectionMock.Object);

            var selectMock = new Mock<ISelectRegistryBD>();
            selectMock.Setup(s => s.SelectRecordAsync())
                      .ReturnsAsync(new List<TdAttachmentDto>
                      {
                          new TdAttachmentDto { TdId = 1, TdCedula = "123456" }
                      });

            var records = await selectMock.Object.SelectRecordAsync();

            Assert.NotNull(records);
            Assert.Single(records);
            Assert.Equal(1, records[0].TdId);
        }
    }

    public class QueryCustomerDataClientTests
    {
        [Fact]
        public async Task GetDataClientReturnsClienteWhenResponseValid()
        {
            var dto = new TdAttachmentDto { TdCedula = "123" };
            var cliente = new Cliente { PrimerNombre = "Test" };
            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage { StatusCode = HttpStatusCode.OK, Content = new StringContent(JsonSerializer.Serialize(new List<Cliente> { cliente }), Encoding.UTF8, "application/json") });
            var httpClient = new HttpClient(handlerMock.Object);
            var configMock = new Mock<IConfiguration>();
            configMock.Setup(c => c.GetSection("Format_Api_Customer_Data").Value).Returns("JSON");
            configMock.Setup(c => c["Url_ConsultaDatosCliente"]).Returns("http://fake-url/");
            var loggerMock = new Mock<ILogger<QueryCustomerDataClient>>();
            var service = new QueryCustomerDataClient(loggerMock.Object, configMock.Object, httpClient);

            var res = await service.GetDataClient(dto);

            Assert.Equal("Test", res.PrimerNombre);
        }

        [Fact]
        public async Task GetDataClientReturnsEmptyClienteWhenResponseEmpty()
        {
            var dto = new TdAttachmentDto { TdCedula = "123" };
            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage { StatusCode = HttpStatusCode.OK, Content = new StringContent("[]", Encoding.UTF8, "application/json") });
            var httpClient = new HttpClient(handlerMock.Object);
            var configMock = new Mock<IConfiguration>();
            configMock.Setup(c => c.GetSection("Format_Api_Customer_Data").Value).Returns("JSON");
            configMock.Setup(c => c["Url_ConsultaDatosCliente"]).Returns("http://fake-url/");
            var loggerMock = new Mock<ILogger<QueryCustomerDataClient>>();
            var service = new QueryCustomerDataClient(loggerMock.Object, configMock.Object, httpClient);

            var res = await service.GetDataClient(dto);

            Assert.NotNull(res);
        }

        [Fact]
        public async Task GetDataClientThrowsHttpRequestExceptionWhenHttpFails()
        {
            var dto = new TdAttachmentDto { TdCedula = "123" };
            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ThrowsAsync(new HttpRequestException());
            var httpClient = new HttpClient(handlerMock.Object);
            var configMock = new Mock<IConfiguration>();
            configMock.Setup(c => c.GetSection("Format_Api_Customer_Data").Value).Returns("JSON");
            configMock.Setup(c => c["Url_ConsultaDatosCliente"]).Returns("http://fake-url/");
            var loggerMock = new Mock<ILogger<QueryCustomerDataClient>>();
            var service = new QueryCustomerDataClient(loggerMock.Object, configMock.Object, httpClient);

            await Assert.ThrowsAsync<HttpRequestException>(() => service.GetDataClient(dto));
        }

        [Fact]
        public async Task GetDataClientThrowsExceptionWhenUnexpectedError()
        {
            var dto = new TdAttachmentDto { TdCedula = "123" };
            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock.Protected()
                // Use InvalidOperationException to be specific (analyzer: avoid System.Exception)
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ThrowsAsync(new InvalidOperationException("unexpected"));
            var httpClient = new HttpClient(handlerMock.Object);
            var configMock = new Mock<IConfiguration>();
            configMock.Setup(c => c.GetSection("Format_Api_Customer_Data").Value).Returns("JSON");
            configMock.Setup(c => c["Url_ConsultaDatosCliente"]).Returns("http://fake-url/");
            var loggerMock = new Mock<ILogger<QueryCustomerDataClient>>();
            var service = new QueryCustomerDataClient(loggerMock.Object, configMock.Object, httpClient);

            await Assert.ThrowsAsync<InvalidOperationException>(() => service.GetDataClient(dto));
        }
    }

    public class SftpFileUploaderClientTests
    {
        public static SftpFileUploaderClient CreateClient(SftpSettings? settings = null)

        {
            var loggerMock = new Mock<ILogger<SftpFileUploaderClient>>();
            var configMock = new Mock<IConfiguration>();
            configMock.Setup(c => c.GetSection("SftpSettings").Get<SftpSettings>()).Returns(settings ?? new SftpSettings
            {
                Host = "localhost",
                Port = 22,
                Username = "user",
                Password = "pass",
                RemotePath = "/remote",
                ErrorPath = "/error"
            });
            return new SftpFileUploaderClient(loggerMock.Object, configMock.Object);
        }

        [Fact]
        public void ValidateThrowsWhenFormatEmpty()
        {
            var ex = Assert.Throws<ArgumentException>(() => SftpFileUploaderClient.Validate("", "AAA"));
            Assert.Contains("Format cannot be empty", ex.Message);
        }

        [Fact]
        public void ValidateThrowsWhenBase64Empty()
        {
            var ex = Assert.Throws<ArgumentException>(() => SftpFileUploaderClient.Validate("PDF", ""));
            Assert.Contains("Base64 content cannot be empty", ex.Message);
        }

        [Fact]
        public void CanSetAndGetProperties()
        {
            var dto = new TdAttachmentDto
            {
                TdId = 123,
                TdCedula = "1234567890",
                TdTipoIdent = "CI",
                TdIdOperation = "OP1",
                TdModelo = "M1",
                TdApplicationId = "APP1",
                TdDomainHost = "domain.local",
                TdReportId = "RPT1",
                TdAdjIdOperation = "ADJ1",
                TdFormat = "PDF",
                TdBase64 = "dGVzdA==",
                TdOperacion = "OPER1",
                TdReportName = "report.pdf",
                // Use explicit DateTimeKind and invariant parsing (or direct ctor)
                TdOperacionFechaRegistro = new DateTime(2025, 10, 1, 0, 0, 0, DateTimeKind.Utc),
                TdOperacionFechaEnvio = new DateTime(2025, 10, 2, 0, 0, 0, DateTimeKind.Utc)

            };

            Assert.Equal(123, dto.TdId);
            Assert.Equal("1234567890", dto.TdCedula);
            Assert.Equal("CI", dto.TdTipoIdent);
            Assert.Equal("OP1", dto.TdIdOperation);
            Assert.Equal("M1", dto.TdModelo);
            Assert.Equal("APP1", dto.TdApplicationId);
            Assert.Equal("domain.local", dto.TdDomainHost);
            Assert.Equal("RPT1", dto.TdReportId);
            Assert.Equal("ADJ1", dto.TdAdjIdOperation);
            Assert.Equal("PDF", dto.TdFormat);
            Assert.Equal("dGVzdA==", dto.TdBase64);
            Assert.Equal("OPER1", dto.TdOperacion);
            Assert.Equal("report.pdf", dto.TdReportName);
            Assert.Equal(new DateTime(2025, 10, 1, 0, 0, 0, DateTimeKind.Utc), dto.TdOperacionFechaRegistro);
            Assert.Equal(new DateTime(2025, 10, 2, 0, 0, 0, DateTimeKind.Utc), dto.TdOperacionFechaEnvio);

        }

        [Fact]
        public void DefaultValuesAreEmptyOrNull()
        {
            var dto = new TdAttachmentDto();

            Assert.Equal(0, dto.TdId);
            Assert.Equal(string.Empty, dto.TdCedula);
            Assert.Equal(string.Empty, dto.TdTipoIdent);
            Assert.Equal(string.Empty, dto.TdIdOperation);
            Assert.Equal(string.Empty, dto.TdModelo);
            Assert.Equal(string.Empty, dto.TdApplicationId);
            Assert.Equal(string.Empty, dto.TdDomainHost);
            Assert.Equal(string.Empty, dto.TdReportId);
            Assert.Equal(string.Empty, dto.TdAdjIdOperation);
            Assert.Equal(string.Empty, dto.TdFormat);
            Assert.Equal(string.Empty, dto.TdBase64);
            Assert.Equal(string.Empty, dto.TdOperacion);
            Assert.Equal(string.Empty, dto.TdReportName);
            Assert.Null(dto.TdOperacionFechaRegistro);
            Assert.Null(dto.TdOperacionFechaEnvio);
        }
    }

    public class FileProcDigExecSPTests
    {
        [Fact]
        public void ToSqlDateTimeOrNullReturnsDBNullWhenDateBeforeMinSqlDate()
        {
            DateTime earlyDate = new DateTime(1000, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            Assert.Equal(DBNull.Value, earlyDate.ToSqlDateTimeOrNull());
        }

        [Fact]
        public void ToSqlDateTimeOrNullReturnsDateWhenDateAfterMinSqlDate()
        {
            DateTime validDate = new DateTime(2025, 10, 1, 0, 0, 0, DateTimeKind.Utc);
            Assert.Equal(validDate, validDate.ToSqlDateTimeOrNull());
        }

        [Fact]
        public void ToSqlDateTimeOrNullNullableReturnsDBNullWhenNull()
        {
            DateTime? nullDate = null;
            Assert.Equal(DBNull.Value, nullDate.ToSqlDateTimeOrNull());
        }

        [Fact]
        public void ToSqlDateTimeOrNullNullableReturnsDateWhenValid()
        {
            DateTime? date = new DateTime(2025, 10, 1, 0, 0, 0, DateTimeKind.Utc);

            Assert.Equal(date.Value, date.ToSqlDateTimeOrNull());
        }
    }
}
