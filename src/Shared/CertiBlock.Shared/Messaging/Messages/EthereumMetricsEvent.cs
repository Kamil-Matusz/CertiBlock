using CertiBlock.Shared.Enums;

namespace CertiBlock.Shared.Messaging.Messages;

public sealed record EthereumMetricsEvent(Guid CertificateId, Blockchain Blockchain, Operation Operation, string TransactionHash,
    int DataSizeBytes, int Confirmations, decimal TransactionCostUsd, decimal TransactionCostNative, long GasUsed, 
    double GasUtilizationRatio) : BaseEvent(Guid.NewGuid(), DateTime.UtcNow, null, "CertiBlock", "1.0");