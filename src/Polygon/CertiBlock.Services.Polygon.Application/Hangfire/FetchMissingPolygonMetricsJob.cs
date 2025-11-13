using CertiBlock.Services.Polygon.Application.Services.PolygonMetrics;
using CertiBlock.Services.Polygon.Core.Repositories;
using Microsoft.Extensions.Logging;

namespace CertiBlock.Services.Polygon.Application.Hangfire;

public class FetchMissingPolygonMetricsJob(IPolygonRepository polygonRepository, IPolygonMetricRepository polygonMetricRepository,
    IPolygonMetricService polygonMetricService,ILogger<FetchMissingPolygonMetricsJob> logger)
{
    public async Task FetchMissingMetricsAsync()
    {
        var allCertificateIds = await polygonRepository.GetAllCertificateIdsAsync();
        var allCertificateIdsList = allCertificateIds.ToList();
        
        var certificateIdsWithMetrics = await polygonMetricRepository.GetAllCertificateIdsWithMetricsAsync();
        var certificateIdsWithMetricsSet = certificateIdsWithMetrics.ToHashSet();
        
        var certificateIdsWithoutMetrics = allCertificateIdsList
            .Where(certId => !certificateIdsWithMetricsSet.Contains(certId))
            .ToList();

        if (!certificateIdsWithoutMetrics.Any())
        {
            logger.LogInformation("All certificates have metrics. Nothing to fetch.");
            return;
        }
        
        int fetchedCount = 0;
        int failedCount = 0;

        foreach (var certificateId in certificateIdsWithoutMetrics)
        {
            try
            {
                var transaction = await polygonRepository.GetBlockchainTransactionByCertificateIdAsync(certificateId);

                if (transaction is null)
                {
                    logger.LogWarning("Transaction not found for certificate {CertificateId}", certificateId);
                    failedCount++;
                    continue;
                }

                logger.LogInformation("Collecting metrics for certificate {CertificateId}, transaction {TransactionHash}",
                    certificateId, transaction.TransactionHash);
                
                await polygonMetricService.CollectMetricsAsync(certificateId, transaction.TransactionHash);
                    
                fetchedCount++;
                logger.LogInformation("Successfully collected and saved metrics for certificate {CertificateId}", certificateId);
                
                await Task.Delay(500);
            }
            catch (Exception ex)
            {
                failedCount++;
                logger.LogError(ex, "Error collecting metrics for certificate {CertificateId}", certificateId);
            }
        }
        
        logger.LogInformation(
            "Job completed. Fetched: {FetchedCount}, Failed: {FailedCount}, 📋 Total Missing: {TotalMissing}", fetchedCount,
            failedCount, certificateIdsWithoutMetrics.Count);
    }
}