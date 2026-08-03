namespace CertiBlock.Shared.CORS;

public class CorsSettings
{
    public string[] AllowedOrigins { get; set; }
    public string[] AllowedMethods { get; set; }
    public string[] AllowedHeaders { get; set; }
}