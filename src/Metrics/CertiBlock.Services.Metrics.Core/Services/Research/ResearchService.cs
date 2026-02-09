using System.Net.Http.Json;
using CertiBlock.Shared.DTO;
using Microsoft.Extensions.Logging;

namespace CertiBlock.Services.Metrics.Core.Services.Research;

public class ResearchService(IHttpClientFactory httpClientFactory, ILogger<ResearchService> logger) : IResearchService
{
    public async Task<IEnumerable<ResearchMetricDto>> GetComparativeResearchMetricsAsync()
    {
        var ethereumTask = FetchResearchMetricsAsync("EthereumService", "ethereumMetrics/getEthereumResearchMetrics");
        var polygonTask = FetchResearchMetricsAsync("PolygonService", "polygonMetrics/getPolygonResearchMetrics");

        await Task.WhenAll(ethereumTask, polygonTask);

        var results = new List<ResearchMetricDto>();
        results.AddRange(ethereumTask.Result);
        results.AddRange(polygonTask.Result);

        logger.LogInformation("Fetched comparative research metrics: {EthCount} Ethereum + {PolyCount} Polygon",
            ethereumTask.Result.Count, polygonTask.Result.Count);

        return results;
    }

    private async Task<List<ResearchMetricDto>> FetchResearchMetricsAsync(string clientName, string endpoint)
    {
        try
        {
            var client = httpClientFactory.CreateClient(clientName);
            var response = await client.GetAsync(endpoint);
            response.EnsureSuccessStatusCode();

            var metrics = await response.Content.ReadFromJsonAsync<List<ResearchMetricDto>>();
            return metrics ?? [];
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error fetching research metrics from {ClientName}", clientName);
            return [];
        }
    }
}
