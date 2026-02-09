using CertiBlock.Shared.Messaging;

namespace CertiBlock.Services.Metrics.Core.Services;

public interface IMetricsService
{
    Task WriteBlockchainMetricAsync(MetricCollectedEvent metricEvent);
    Task WriteFinalizationMetricAsync(MetricFinalizedEvent metricEvent);
}