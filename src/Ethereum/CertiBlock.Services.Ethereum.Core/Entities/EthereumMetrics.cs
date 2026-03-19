using CertiBlock.Shared.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CertiBlock.Services.Ethereum.Core.Entities;

public class EthereumMetrics
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid CertificateId { get; set; }
    public Blockchain Blockchain { get; set; }
    public Operation Operation { get; set; }
    public string TransactionHash { get; set; }
    public int DataSizeBytes { get; set; }
    public int Confirmations { get; set; }
    public decimal TransactionCostUsd { get; set; }
    public decimal TransactionCostNative { get; set; }
    public long GasUsed { get; set; }
    public double GasUtilizationRatio { get; set; }
    public double InclusionTimeSeconds { get; set; }
    public long BlockNumber { get; set; }
    public double? FinalizationTimeSeconds { get; set; }
    public bool IsFinalized { get; set; } = false;
    public DateTime CollectedAt { get; set; } = DateTime.UtcNow;
}