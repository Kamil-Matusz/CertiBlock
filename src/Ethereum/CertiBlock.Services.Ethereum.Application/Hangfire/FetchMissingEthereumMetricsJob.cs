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
        var allCertificateTransactions = await ethereumRepository.GetConfirmedCertificateTransactionsAsync();
        var certificateIdsWithMetrics = (await ethereumMetricRepository.GetAllCertificateIdsWithMetricsAsync()).ToHashSet();
        
        var missingCertificates = allCertificateTransactions
            .Where(kvp => !certificateIdsWithMetrics.Contains(kvp.Key))
            .ToList();

        if (!missingCertificates.Any())
        {
            logger.LogInformation("All certificates have metrics. Nothing to fetch.");
            return;
        }
        
        logger.LogInformation("Found {Count} certificates without metrics. Starting collection...", missingCertificates.Count);
        
        int fetchedCount = 0;
        int failedCount = 0;

        foreach (var (certificateId, transactionHash) in missingCertificates)
        {
            try
            {
                logger.LogInformation("Collecting metrics for certificate {CertificateId}, transaction {TransactionHash}",
                    certificateId, transactionHash);
                
                await ethereumMetricService.CollectMetricsAsync(certificateId, transactionHash);
                    
                fetchedCount++;
                logger.LogInformation("Successfully collected metrics for certificate {CertificateId}", certificateId);
                
                await Task.Delay(500);
            }
            catch (Exception ex)
            {
                failedCount++;
                logger.LogError(ex, "Error collecting metrics for certificate {CertificateId}", certificateId);
            }
        }
        
        logger.LogInformation(
            "Job completed. Fetched: {FetchedCount}, Failed: {FailedCount}, Total Missing: {TotalMissing}", 
            fetchedCount, failedCount, missingCertificates.Count);
    }
}