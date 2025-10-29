

namespace SendSPFT.Installers
{
    using System.Data.CData.Sybase;
    using System.Text.Json;

    using Amazon;
    using Amazon.SimpleNotificationService;
    using Amazon.SQS;



    using Davivienda.Framework.Lib.BitacoraLog.BitacoraLog;
    using Davivienda.Framework.Lib.BitacoraLog.MQAccess;
    using Davivienda.Framework.Lib.Crypto;
    using Davivienda.Framework.Lib.EventBusConfig;

    using Davivienda.Framework.Lib.VoucherNumber;

    using Prometheus.SystemMetrics;

    using SendSPFT.Application.Clients;
    using SendSPFT.Application.Contracts.Clients;
    using SendSPFT.Application.Contracts.Services;
    using SendSPFT.Application.Services;
    using SendSPFT.CrossCutting.Healthchecks;
    using SendSPFT.CrossCutting.Helpers;
    using SendSPFT.CrossCutting.Helpers.Contracts;
    using SendSPFT.CrossCutting.Mapper;
    using SendSPFT.CrossCutting.Metrics;
    using SendSPFT.CrossCutting.Subservices;
    using SendSPFT.CrossCutting.Subservices.ErrorSftp;
    using SendSPFT.DataAccess.Context;
    using SendSPFT.DataAccess.Contracts.Context;
    using SendSPFT.DataAccess.Contracts.Exec;
    using SendSPFT.DataAccess.Contracts.Exec.Func;
    using SendSPFT.DataAccess.Contracts.Exec.SP;
    using SendSPFT.DataAccess.Exec.Func;
    using SendSPFT.DataAccess.Exec.SP;

    public static class ServiceExtensions
    {
        public static void RegisterServices(this IServiceCollection services, IConfiguration configuration)
        {
           

            services.AddTransient<ISendSftpService, SendSftpService>();
            services.AddTransient<ISftpFileUploader, SftpFileUploaderClient>();
            services.AddTransient<IAttachmentMapper, AttachmentMapper>();
            services.AddHttpClient<IQueryCustomerDataClient, QueryCustomerDataClient>();
            services.AddTransient<ISelectRegistryBD, SelectRegistryBD>();
            services.AddTransient<IFileProcDigExecSP, FileProcDigExecSP>();
            services.AddTransient<IUpdateTableBD, UpdateTableBD>();
            services.AddTransient<IAttachmentMapper, AttachmentMapper>();
            services.AddTransient<IFileRegBitacoraExexSP, FileRegBitacoraExexSP>();
            services.Configure<EventBusConfig>(configuration.GetSection("EventBusConfig"));
            services.AddSingleton<IConnectionMQAcess, ConnectionMQAcess>();
            services.AddSingleton<IBitacoraLog , BitacoraLog>();
            services.AddSingleton<IVoucherNumber , VoucherNumber>();
            services.AddSingleton<ICrypto, Crypto>();
            services.AddTransient<MetricCollector>();
            services.AddScoped<IRequestContextHeadersExtractor, RequestContextHeadersExtractor>();
            services.AddScoped<IRequestLogger, RequestLogger>();
            services.AddScoped<IServiceResponseFactory, ServiceResponseFactory>();
 
            services.AddTransient<ISendSftpContext, SendSftpContext>();
            services.AddTransient<IGetDatesSyb12, GetDatesSyb12>();
            services.AddTransient<IFileProcessingService, FileProcessingService>();
            services.AddTransient<IProcessRecord, ProcessRecord>();
            services.AddTransient<IBdSybase, BdSybase>();
            services.AddTransient<IErrorFileService, ErrorFileService>();

           
            






            services.AddSingleton<IAmazonSQS>(sp =>
            {
                var config = sp.GetRequiredService<IConfiguration>();
                var awsConfig = new AmazonSQSConfig
                {
                    RegionEndpoint = RegionEndpoint.GetBySystemName(config["AWS:Region"]),
                    ServiceURL = config["AWS:SQS:Endpoint"],
                    UseHttp = true
                };


                return new AmazonSQSClient(awsConfig);
            });

            services.AddSingleton<IAmazonSimpleNotificationService>(sp =>
            {
                var config = sp.GetRequiredService<IConfiguration>();
                var snsConfig = new AmazonSimpleNotificationServiceConfig
                {
                    RegionEndpoint = RegionEndpoint.GetBySystemName(config["AWS:Region"]),
                    ServiceURL = config["AWS:SNS:Endpoint"],
                    UseHttp = true
                };

                return new AmazonSimpleNotificationServiceClient(snsConfig);
            });
            services.AddSystemMetrics();


        }
 
       

    }
}

