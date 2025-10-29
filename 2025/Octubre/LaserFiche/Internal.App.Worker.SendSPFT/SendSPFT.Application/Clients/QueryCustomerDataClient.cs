namespace SendSPFT.Application.Clients
{

    using System.Text;
    using System.Text.Json;
    using System.Threading.Tasks;

    using Core.Customer.Web.Api.ConsultaDatosClientes.Business.Models;

    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;

    using SendSPFT.Application.Contracts.Clients;
    using SendSPFT.Business.Models.ApiQCD;
    using SendSPFT.Business.Models.BD;

    using static System.Net.WebRequestMethods;

    public class QueryCustomerDataClient(ILogger<QueryCustomerDataClient> serilog, IConfiguration configuration, HttpClient httpClient) : IQueryCustomerDataClient
    {
        private static readonly JsonSerializerOptions _jsonOptions =
             new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        #region
        private static readonly Action<ILogger, string, Exception?> _logResponseReceived =
       LoggerMessage.Define<string>(
           LogLevel.Information,
           new EventId(1001, nameof(_logResponseReceived)),
           "Received response: {ResponseJson}");

        private static readonly Action<ILogger, string, Exception?> _logHttpError =
            LoggerMessage.Define<string>(
                LogLevel.Error,
                new EventId(2001, nameof(_logHttpError)),
                "HTTP request failed while fetching client data for ID {ClientId}");

        private static readonly Action<ILogger, string, Exception?> _logUnexpectedError =
            LoggerMessage.Define<string>(
                LogLevel.Error,
                new EventId(2002, nameof(_logUnexpectedError)),
                "Unexpected error while fetching client data for ID {ClientId}");

        #endregion
        public async Task<Cliente> GetDataClient(TdAttachmentDto dto)
        {


            try
            {

                var requestWS = new ReqConsulta
                {
                    nroIdentificacion = dto.TdCedula,
                    tipoIdentificacion = dto.TdTipoIdent,
                    conFormato = configuration.GetSection("Format_Api_Customer_Data").Value
                };

                string jsonPayload = JsonSerializer.Serialize(requestWS);


                string url = configuration["Url_ConsultaDatosCliente"] ?? string.Empty;


                var requestMessage = new HttpRequestMessage(HttpMethod.Post, url)
                {
                    Content = new StringContent(jsonPayload, Encoding.UTF8, "application/json")
                };


                HttpResponseMessage response = await httpClient.SendAsync(requestMessage);
                response.EnsureSuccessStatusCode();




                string responseContent = await response.Content.ReadAsStringAsync();
               



                var clientes = JsonSerializer.Deserialize<List<Cliente>>(responseContent, _jsonOptions);
                _logResponseReceived(serilog, clientes?[0].Ente ?? string.Empty , null);

                return clientes?.FirstOrDefault() ?? new Cliente();

            }
            catch (HttpRequestException httpEx)
            {
                _logHttpError(serilog, httpEx.Message, null);
                throw;
            }
            catch (Exception ex)
            {
                _logHttpError(serilog, ex.Message, null);
                throw;
            }

        }
           


        
    }
}
