

namespace SendSPFT.Application.Contracts.Services
{
  
    public interface ISendSftpService
    {
        Task ExecuteServiceAsync(CancellationToken stoppingToken);
    }
}

