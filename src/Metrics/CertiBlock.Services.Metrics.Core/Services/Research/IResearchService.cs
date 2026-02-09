using CertiBlock.Shared.DTO;

namespace CertiBlock.Services.Metrics.Core.Services.Research;

public interface IResearchService
{
    Task<IEnumerable<ResearchMetricDto>> GetComparativeResearchMetricsAsync();
}
