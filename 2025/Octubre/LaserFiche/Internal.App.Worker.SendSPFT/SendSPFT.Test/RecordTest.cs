namespace SendSPFT.Test
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;

    using Moq;

    using SendSPFT.Application.Contracts.Clients;
    using SendSPFT.Business.Models.BD;
    using SendSPFT.CrossCutting.Subservices;
    using SendSPFT.CrossCutting.Subservices.ErrorSftp;
    using SendSPFT.DataAccess.Contracts.Exec.Func;

    public class RecordTest
    {

        [Fact]
        public async Task ProcessRecordAsyncDoesNotThrowWhenExceptionThrown()
        {
           
            var loggerMock = new Mock<ILogger<ProcessRecord>>();
            var queryMock = new Mock<IQueryCustomerDataClient>();
            var rdsMock = new Mock<IUpdateTableBD>();
            var fileProcessingMock = new Mock<IFileProcessingService>();
            var configMock = new Mock<IConfiguration>();
            var fileError = new Mock<IErrorFileService>();

            
            queryMock
                .Setup(q => q.GetDataClient(It.IsAny<TdAttachmentDto>()))
                .ThrowsAsync(new InvalidOperationException("Test Exception"));

            var record = new TdAttachmentDto { TdId = 123 };

            var service = new ProcessRecord(
                loggerMock.Object,
                queryMock.Object,
                rdsMock.Object,
                fileProcessingMock.Object,
                configMock.Object,
                fileError.Object

            );

          
            var exception = await Record.ExceptionAsync(() => service.ProcessRecordAsync(record));

            Assert.Null(exception); 
        }

    }
}
