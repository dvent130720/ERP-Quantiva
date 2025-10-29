namespace SendSPFT.CrossCutting.Subservices
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using Core.Customer.Web.Api.ConsultaDatosClientes.Business.Models;

    using SendSPFT.Application.Contracts.Clients;
    using SendSPFT.Business.Models.BD;
    using SendSPFT.CrossCutting.Mapper;
    using SendSPFT.DataAccess.Contracts.Context;
    using SendSPFT.DataAccess.Contracts.Exec.SP;

    public class FileProcessingService : IFileProcessingService
    {
        private readonly ISftpFileUploader _uploader;
        private readonly IFileProcDigExecSP _spService;
        private readonly IAttachmentMapper _mapper;

        public FileProcessingService(
            ISftpFileUploader uploader,
            IFileProcDigExecSP spService,
            IAttachmentMapper mapper)
        {
            _uploader = uploader;
            _spService = spService;
            _mapper = mapper;
        }

        public async Task ProcessAsync(TdAttachmentDto record, Cliente data, GetDatesSyb12ResponseDto dates)
        {
            var result = await _uploader.UploadFileAsync(record.TdFormat, record.TdBase64, record.TdReportName);
            if (result.Success)
            {
            
                
                var procDto = _mapper.MapToProcDig(data, result, record, dates);
                await _spService.ExecuteFileProcDigAsync(procDto);
            }
        }
    }
}
