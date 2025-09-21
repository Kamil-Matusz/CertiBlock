namespace CertiBlock.Services.Polygon.Core.DTO;

public class PolygonBalanceDto
{
    public string Address { get; set; }
    public decimal Balance { get; set; }
    public string Unit { get; set; } = "POL";
}