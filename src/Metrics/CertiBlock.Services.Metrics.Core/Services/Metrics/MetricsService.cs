using CertiBlock.Shared.InfluxDB;
using CertiBlock.Shared.Messaging;
using InfluxDB.Client;
using InfluxDB.Client.Api.Domain;
using InfluxDB.Client.Writes;
using Microsoft.Extensions.Logging;

namespace CertiBlock.Services.Metrics.Core.Services.Metrics;

public class MetricsService(IInfluxDBClient client, InfluxDbOptions options, ILogger<MetricsService> logger) : IMetricsService
{
    public async Task WriteBlockchainMetricAsync(MetricCollectedEvent metricEvent)
    {
        try
        {
            var writeApi = client.GetWriteApiAsync();

            var point = PointData
                .Measurement("blockchain_operations")
                .Tag("blockchain", metricEvent.Blockchain.ToString())
                .Tag("operation", metricEvent.Operation.ToString())
                .Tag("certificate_id", metricEvent.CertificateId.ToString())
                .Field("gas_used", metricEvent.GasUsed)
                .Field("inclusion_time_seconds", metricEvent.TransactionTime)
                .Field("transaction_cost_usd", metricEvent.TransactionCostUsd)
                .Timestamp(metricEvent.Timestamp, WritePrecision.Ns);

            await writeApi.WritePointAsync(point, options.Bucket, options.Organization);
            
            logger.LogInformation("Blockchain metric written: {Blockchain} - {Operation}", 
                                   metricEvent.Blockchain, metricEvent.Operation);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error writing blockchain metric");
        }
    }

    public async Task WriteFinalizationMetricAsync(MetricFinalizedEvent metricEvent)
    {
        try
        {
            var writeApi = client.GetWriteApiAsync();

            var point = PointData
                .Measurement("blockchain_finalization")
                .Tag("blockchain", metricEvent.Blockchain.ToString())
                .Tag("certificate_id", metricEvent.CertificateId.ToString())
                .Field("finalization_time_seconds", metricEvent.FinalizationTimeSeconds)
                .Field("confirmations", metricEvent.Confirmations)
                .Timestamp(metricEvent.Timestamp, WritePrecision.Ns);

            await writeApi.WritePointAsync(point, options.Bucket, options.Organization);

            logger.LogInformation("Finalization metric written: {Blockchain} - CertificateId: {CertificateId}",
                                   metricEvent.Blockchain, metricEvent.CertificateId);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error writing finalization metric");
        }
    }
}