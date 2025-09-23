using CertiBlock.Services.Polygon.Application.Mappers;

namespace CertiBlock.Services.Polygon.UnitTests.Mappers;

public class PolygonBalanceMapperTests
{
    [Fact]
    public void MapToDto_WithValidAddressAndBalance_ShouldReturnCorrectDto()
    {
        // Arrange
        var address = "0x1234567890abcdef1234567890abcdef12345678";
        var balance = 1.5m;

        // Act
        var result = PolygonBalanceMapper.MapToDto(address, balance);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(address, result.Address);
        Assert.Equal(balance, result.Balance);
        Assert.Equal("MATIC", result.Unit);
    }
    
    [Fact]
    public void MapToDto_ShouldCreateNewInstanceEachTime()
    {
        // Arrange
        var address = "0x1234567890abcdef1234567890abcdef12345678";
        var balance = 1.5m;

        // Act
        var result1 = PolygonBalanceMapper.MapToDto(address, balance);
        var result2 = PolygonBalanceMapper.MapToDto(address, balance);

        // Assert
        Assert.NotSame(result1, result2);
        Assert.Equal(result1.Address, result2.Address);
        Assert.Equal(result1.Balance, result2.Balance);
        Assert.Equal(result1.Unit, result2.Unit);
    }
}