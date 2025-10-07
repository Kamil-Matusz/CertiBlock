namespace CertiBlock.Shared.Messaging.Messages;

public sealed record EthereumMetricsCollected(
    Guid EventId,
    DateTime Timestamp,
    Guid MetricsId,
    Guid CertificateId,
    string Blockchain,
    string Operation,
    string TransactionHash,
    int DataSizeBytes,
    int Confirmations,
    decimal TransactionCostUsd,
    decimal TransactionCostNative,
    long GasUsed,
    double GasUtilizationRatio,
    string Source,
    string Version
) : IMessage;