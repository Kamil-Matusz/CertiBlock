using InfluxDB.Client;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace CertiBlock.Shared.InfluxDB;

public class InfluxDbHealthCheck(IInfluxDBClient client) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            var isReady = await client.PingAsync();
            return isReady
                ? HealthCheckResult.Healthy()
                : HealthCheckResult.Unhealthy("InfluxDB did not respond to ping.");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("InfluxDB ping failed.", ex);
        }
    }
}