namespace CertiBlock.Shared.DTO;

public class ResearchMetricDto
{
    public Guid CertificateId { get; set; }
    public string TransactionHash { get; set; }
    public decimal TransactionCostNative { get; set; }
    public decimal TransactionCostUsd { get; set; }
    public double? FinalizationTimeSeconds { get; set; }
    public int Confirmations { get; set; }
    public long GasUsed { get; set; }
    public double InclusionTimeSeconds { get; set; }
}
