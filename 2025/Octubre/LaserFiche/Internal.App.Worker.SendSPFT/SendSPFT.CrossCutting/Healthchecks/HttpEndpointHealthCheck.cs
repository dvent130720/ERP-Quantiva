namespace SendSPFT.CrossCutting.Healthchecks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using Microsoft.Extensions.Diagnostics.HealthChecks;

    public class HttpEndpointHealthCheck (IHttpClientFactory httpClientFactory, string url) : IHealthCheck
    {
       

        public async Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var client = httpClientFactory.CreateClient();
                var response = await client.GetAsync(url, cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    return HealthCheckResult.Healthy("Servicio HTTP disponible");
                }

                return HealthCheckResult.Unhealthy($"Código de respuesta: {response.StatusCode}");
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy($"Excepción: {ex.Message}");
            }
        }
    }

}

