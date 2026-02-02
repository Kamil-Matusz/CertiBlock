using CertiBlock.Services.Users.Core.Entities;
using CertiBlock.Services.Users.Core.ValueObjects;

namespace CertiBlock.Services.Users.Core.Repositories;

public interface IUserRepository
{
    Task<User> GetUserByIdAsync(Guid userId);
    Task<User> GetUserByEmailAsync(string email);
    Task AddUserAsync(User user);
    Task<bool> CheckAccountActivity(string email);
    Task DeleteUserAsync(User user);
    Task ChangeUserRoleAsync(Guid userId, Role role);
    Task ChangeAccountStatusAsync(Guid userId, bool status);
    Task ChangeUserPassword(Guid userId, string password);
}