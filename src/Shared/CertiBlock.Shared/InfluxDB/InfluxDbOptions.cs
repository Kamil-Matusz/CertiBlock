namespace CertiBlock.Shared.InfluxDB;

public class InfluxDbOptions
{
    public string Url { get; set; }
    public string Token { get; set; }
    public string Organization { get; set; }
    public string Bucket { get; set; }
}