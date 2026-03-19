using CertiBlock.Shared.Enums;

namespace CertiBlock.Services.Ethereum.Core.DTO;

public class EthereumMetricDetailsDto
{
    public Guid Id { get; set; }
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
    public bool IsFinalized { get; set; }
    public DateTime CollectedAt { get; set; }
}