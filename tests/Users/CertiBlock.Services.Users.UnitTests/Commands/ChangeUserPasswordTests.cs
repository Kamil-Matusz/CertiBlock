using CertiBlock.Services.Users.Application.Commands;
using Shouldly;

namespace CertiBlock.Services.Users.UnitTests.Commands;

public class ChangeUserPasswordTests
{
    [Fact]
    public void changeUserPassword_shouldSetProperties_correctly()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var password = "NewSecurePassword123!";

        // Act
        var command = new ChangeUserPassword(userId, password);

        // Assert
        command.UserId.ShouldBe(userId);
        command.Password.ShouldBe(password);
    }
}