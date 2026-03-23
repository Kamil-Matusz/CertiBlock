using CertiBlock.Shared.RabbitMQ;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace CertiBlock.Services.Ethereum.Application.RabbitMQ;

public class MetricPublisher(IConnection connection, ILogger<MetricPublisher> logger)
    : MetricPublisherBase(connection, logger, "certiblock.metrics.ethereum", "certiblock.finalization.ethereum");
