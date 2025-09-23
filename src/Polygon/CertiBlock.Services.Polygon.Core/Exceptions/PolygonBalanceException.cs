using CertiBlock.Shared.Exceptions;

namespace CertiBlock.Services.Polygon.Core.Exceptions;

public class PolygonBalanceException(string address) : CustomException($"Failed to get Polygon balance for address: '{address}")
{
    public string Address { get; } = address;
}