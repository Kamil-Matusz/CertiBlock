using CertiBlock.Services.Ethereum.Application.Mappers;

namespace CertiBlock.Services.Ethereum.UnitTests.Mappers;

public class EthBalanceMapperTests
{
    [Fact]
    public void MapToDto_WithValidAddressAndBalance_ShouldReturnCorrectDto()
    {
        // Arrange
        var address = "0x1234567890abcdef1234567890abcdef12345678";
        var balance = 1.5m;

        // Act
        var result = EthBalanceMapper.MapToDto(address, balance);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(address, result.Address);
        Assert.Equal(balance, result.Balance);
        Assert.Equal("ETH", result.Unit);
    }
    
    [Fact]
    public void MapToDto_ShouldCreateNewInstanceEachTime()
    {
        // Arrange
        var address = "0x1234567890abcdef1234567890abcdef12345678";
        var balance = 1.5m;

        // Act
        var result1 = EthBalanceMapper.MapToDto(address, balance);
        var result2 = EthBalanceMapper.MapToDto(address, balance);

        // Assert
        Assert.NotSame(result1, result2);
        Assert.Equal(result1.Address, result2.Address);
        Assert.Equal(result1.Balance, result2.Balance);
        Assert.Equal(result1.Unit, result2.Unit);
    }
}