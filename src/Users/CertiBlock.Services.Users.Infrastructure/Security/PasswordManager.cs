using CertiBlock.Services.Users.Application.Security;
using CertiBlock.Services.Users.Core.Entities;
using Microsoft.AspNetCore.Identity;

namespace CertiBlock.Services.Users.Infrastructure.Security;

internal sealed class PasswordManager(IPasswordHasher<User> passwordHasher) : IPasswordManager
{
    public string Secure(string password) => passwordHasher.HashPassword(default, password);

    public bool Validate(string password, string securedPassword) =>
        passwordHasher.VerifyHashedPassword(default, securedPassword, password) is PasswordVerificationResult.Success;
}