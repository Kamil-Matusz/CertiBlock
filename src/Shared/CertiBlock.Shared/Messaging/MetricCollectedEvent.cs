using CertiBlock.Shared.Enums;

namespace CertiBlock.Shared.Messaging;

public record MetricCollectedEvent(Guid CertificateId, Blockchain Blockchain, Operation Operation, double GasUsed, 
    double TransactionTime, double TransactionFee, DateTime Timestamp);