using CertiBlock.Services.Users.Application.Commands;
using Shouldly;

namespace CertiBlock.Services.Users.UnitTests.Commands;

public class DeleteUserAccountTests
{
    [Fact]
    public void deleteUserAccount_shouldSetUserId_correctly()
    {
        // Arrange
        var userId = Guid.NewGuid();

        // Act
        var command = new DeleteUserAccount(userId);

        // Assert
        command.UserId.ShouldBe(userId);
    }
}