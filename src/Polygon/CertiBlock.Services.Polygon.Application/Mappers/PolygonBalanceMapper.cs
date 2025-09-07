using CertiBlock.Services.Polygon.Core.DTO;

namespace CertiBlock.Services.Polygon.Application.Mappers;

public static class PolygonBalanceMapper
{
    public static PolygonBalanceDto MapToDto(string address, decimal balance) => new()
    {
        Address = address,
        Balance = balance,
        Unit = "MATIC"
    };
}