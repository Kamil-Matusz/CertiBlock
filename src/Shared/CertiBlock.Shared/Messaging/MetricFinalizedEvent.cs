using CertiBlock.Shared.Enums;

namespace CertiBlock.Shared.Messaging;

public record MetricFinalizedEvent(Guid CertificateId, Blockchain Blockchain, double FinalizationTimeSeconds,
                                   int Confirmations, DateTime Timestamp);
