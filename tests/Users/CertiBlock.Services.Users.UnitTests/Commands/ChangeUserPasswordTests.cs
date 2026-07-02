using System.Security.Authentication;
using CertiBlock.Services.Users.Application.Commands;
using CertiBlock.Services.Users.Application.Handlers;
using CertiBlock.Services.Users.Application.Security;
using CertiBlock.Services.Users.Core.Entities;
using CertiBlock.Services.Users.Core.Exceptions;
using CertiBlock.Services.Users.Core.Repositories;
using CertiBlock.Services.Users.Core.ValueObjects;
using Moq;
using Shouldly;

namespace CertiBlock.Services.Users.UnitTests.Commands;

public class ChangeUserPasswordTests
{
    private readonly Mock<IPasswordManager> _passwordManager = new();
    private readonly Mock<IUserRepository> _userRepository = new();

    private ChangeUserPasswordHandler CreateHandler()
        => new(_passwordManager.Object, _userRepository.Object);

    private static User CreateUser(Guid userId, string securedPassword)
        => new(userId, "testemail@test.com", securedPassword, Role.User(), true, DateTime.UtcNow);

    [Fact]
    public void changeUserPassword_shouldSetProperties_correctly()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var currentPassword = "OldPassword123!";
        var newPassword = "NewSecurePassword123!";

        // Act
        var command = new ChangeUserPassword(userId, currentPassword, newPassword);

        // Assert
        command.UserId.ShouldBe(userId);
        command.CurrentPassword.ShouldBe(currentPassword);
        command.NewPassword.ShouldBe(newPassword);
    }

    [Fact]
    public async Task changeUserPassword_shouldThrowUserNotFound_whenUserDoesNotExist()
    {
        // Arrange
        var command = new ChangeUserPassword(Guid.NewGuid(), "old", "newPassword123");
        _userRepository.Setup(x => x.GetUserByIdAsync(command.UserId)).ReturnsAsync((User)null!);

        // Act & Assert
        await Should.ThrowAsync<UserNotFoundException>(() => CreateHandler().HandleAsync(command));
    }

    [Fact]
    public async Task changeUserPassword_shouldThrowInvalidCredential_whenCurrentPasswordIsWrong()
    {
        // Arrange
        var command = new ChangeUserPassword(Guid.NewGuid(), "wrongPassword", "newPassword123");
        var user = CreateUser(command.UserId, "securedOldPassword");
        _userRepository.Setup(x => x.GetUserByIdAsync(command.UserId)).ReturnsAsync(user);
        _passwordManager.Setup(x => x.Validate(command.CurrentPassword, user.Password)).Returns(false);

        // Act & Assert
        await Should.ThrowAsync<InvalidCredentialException>(() => CreateHandler().HandleAsync(command));
        _userRepository.Verify(x => x.ChangeUserPassword(It.IsAny<Guid>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task changeUserPassword_shouldChangePassword_whenCurrentPasswordIsValid()
    {
        // Arrange
        var command = new ChangeUserPassword(Guid.NewGuid(), "oldPassword123", "newPassword123");
        var user = CreateUser(command.UserId, "securedOldPassword");
        _userRepository.Setup(x => x.GetUserByIdAsync(command.UserId)).ReturnsAsync(user);
        _passwordManager.Setup(x => x.Validate(command.CurrentPassword, user.Password)).Returns(true);
        _passwordManager.Setup(x => x.Secure(command.NewPassword)).Returns("securedNewPassword");

        // Act
        await CreateHandler().HandleAsync(command);

        // Assert
        _userRepository.Verify(x => x.ChangeUserPassword(command.UserId, "securedNewPassword"), Times.Once);
    }
}
