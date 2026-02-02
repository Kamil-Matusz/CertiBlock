using CertiBlock.Services.Users.Application.Commands;
using Shouldly;

namespace CertiBlock.Services.Users.UnitTests.Commands;

public class SignUpTests
{
    [Fact]
    public void signUp_shouldSetProperties_correctly()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var email = "testemail@test.com";
        var password = "password";
        var role = "Nurse";
        var isActive = true;

        // Act
        var signUpCommand = new SignUp(userId, email, password, role, isActive);

        // Assert
        signUpCommand.UserId.ShouldBe(userId);
        signUpCommand.Email.ShouldBe(email);
        signUpCommand.Password.ShouldBe(password);
        signUpCommand.Role.ShouldBe(role);
        signUpCommand.IsActive.ShouldBe(isActive);
    }
}