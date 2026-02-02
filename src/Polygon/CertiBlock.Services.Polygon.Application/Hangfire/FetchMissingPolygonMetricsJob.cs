using CertiBlock.Services.Polygon.Application.Services.PolygonMetrics;
using CertiBlock.Services.Polygon.Core.Repositories;
using Microsoft.Extensions.Logging;

namespace CertiBlock.Services.Polygon.Application.Hangfire;

public class FetchMissingPolygonMetricsJob(IPolygonRepository polygonRepository, IPolygonMetricRepository polygonMetricRepository,
    IPolygonMetricService polygonMetricService,ILogger<FetchMissingPolygonMetricsJob> logger)
{
    public async Task FetchMissingMetricsAsync()
    {
        var allCertificateTransactions = await polygonRepository.GetConfirmedCertificateTransactionsAsync();
        var certificateIdsWithMetrics = (await polygonMetricRepository.GetAllCertificateIdsWithMetricsAsync()).ToHashSet();
    
        var missingCertificates = allCertificateTransactions
            .Where(kvp => !certificateIdsWithMetrics.Contains(kvp.Key))
            .ToList();

        if (!missingCertificates.Any())
        {
            logger.LogInformation("All certificates have metrics. Nothing to fetch.");
            return;
        }
    
        int fetchedCount = 0;
        int failedCount = 0;

        foreach (var (certificateId, transactionHash) in missingCertificates)
        {
            try
            {
                logger.LogInformation("Collecting metrics for certificate {CertificateId}, transaction {TransactionHash}",
                    certificateId, transactionHash);
            
                await polygonMetricService.CollectMetricsAsync(certificateId, transactionHash);
                fetchedCount++;
            
                await Task.Delay(500);
            }
            catch (Exception ex)
            {
                failedCount++;
                logger.LogError(ex, "Error collecting metrics for certificate {CertificateId}", certificateId);
            }
        }
    
        logger.LogInformation("Job completed. Fetched: {FetchedCount}, Failed: {FailedCount}, Total Missing: {TotalMissing}", 
            fetchedCount, failedCount, missingCertificates.Count);
    }
}