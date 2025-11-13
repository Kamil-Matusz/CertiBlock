using CertiBlock.Services.Ethereum.Application.Services.Ethereum;
using CertiBlock.Services.Ethereum.Application.Services.EthereumMetrics;
using CertiBlock.Services.Ethereum.Core.Repositories;
using Microsoft.Extensions.Logging;

namespace CertiBlock.Services.Ethereum.Application.Hangfire;

public class FetchMissingEthereumMetricsJob(IEthereumRepository ethereumRepository, IEthereumMetricRepository ethereumMetricRepository,
    IEthereumMetricService ethereumMetricService,ILogger<FetchMissingEthereumMetricsJob> logger)
{
    public async Task FetchMissingMetricsAsync()
    {
        var allCertificateIds = await ethereumRepository.GetAllCertificateIdsAsync();
        var allCertificateIdsList = allCertificateIds.ToList();
        
        var certificateIdsWithMetrics = await ethereumMetricRepository.GetAllCertificateIdsWithMetricsAsync();
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
                var transaction = await ethereumRepository.GetBlockchainTransactionByCertificateIdAsync(certificateId);

                if (transaction is null)
                {
                    logger.LogWarning("Transaction not found for certificate {CertificateId}", certificateId);
                    failedCount++;
                    continue;
                }

                logger.LogInformation("Collecting metrics for certificate {CertificateId}, transaction {TransactionHash}",
                    certificateId, transaction.TransactionHash);
                
                await ethereumMetricService.CollectMetricsAsync(certificateId, transaction.TransactionHash);
                    
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