using CertiBlock.Shared.Enums;

namespace CertiBlock.Services.Polygon.Core.DTO;

public class PolygonMetricResearchDto
{
    public Guid Id { get; set; }
    public Guid CertificateId { get; set; }
    public Blockchain Blockchain { get; set; }
    public Operation Operation { get; set; }
    public string TransactionHash { get; set; }
    public int DataSizeBytes { get; set; }
    public int Confirmations { get; set; }
    public long GasUsed { get; set; }
    public decimal TransactionCostUsd { get; set; }
    public double InclusionTimeSeconds { get; set; }
    public long BlockNumber { get; set; }
    public double? FinalizationTimeSeconds { get; set; }
}