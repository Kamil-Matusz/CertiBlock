namespace CertiBlock.Shared.Observability;

public class ObservabilityOptions
{
    public bool Enabled { get; set; }
    public string Endpoint { get; set; }
    public bool IncludeSqlInTraces { get; set; }
}