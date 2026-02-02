using CertiBlock.Services.Users.Application.Services.Clock;

namespace CertiBlock.Services.Users.UnitTests.Services;

public class ClockServiceTests
{
    [Fact]
    public void CurrentDate_ReturnsCurrentUtcDateTime()
    {
        // Arrange
        var clock = new Clock();

        // Act
        var currentDate = clock.CurrentDate();

        // Assert
        var difference = Math.Abs((currentDate - DateTime.UtcNow).TotalSeconds);
        Assert.True(difference < 1);
    }
}