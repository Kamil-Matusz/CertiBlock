using CertiBlock.Services.Users.Core.Entities;
using CertiBlock.Services.Users.Core.Repositories;
using CertiBlock.Services.Users.Core.ValueObjects;
using CertiBlock.Services.Users.Infrastructure.DAL.PostgreSQL;
using Microsoft.EntityFrameworkCore;

namespace CertiBlock.Services.Users.Infrastructure.DAL.Repositories;

internal sealed class UserRepository : IUserRepository
{
    private readonly UsersDbContext _dbContext;
    private readonly DbSet<User> _users;

    public UserRepository(UsersDbContext dbContext)
    {
        _dbContext = dbContext;
        _users = _dbContext.Users;
    }

    public Task<User> GetUserByIdAsync(Guid userId) => _users.SingleOrDefaultAsync(x => x.UserId == userId);

    public Task<User> GetUserByEmailAsync(string email) => _users.SingleOrDefaultAsync(x => x.Email == email);

    public async Task AddUserAsync(User user)
    {
        await _users.AddAsync(user);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<bool> CheckAccountActivity(string email)
    {
        bool isActive = await _users
            .Where(x => x.Email == email)
            .Select(x => x.IsActive)
            .FirstOrDefaultAsync();

        return isActive;
    }

    public async Task DeleteUserAsync(User user)
    {
        _users.Remove(user);
        await _dbContext.SaveChangesAsync();
    }

    public async Task ChangeUserRoleAsync(Guid userId, Role role)
    {
        var user = await _users.SingleOrDefaultAsync(x => x.UserId == userId);
        user.Role = role;

        _users.Update(user);
        await _dbContext.SaveChangesAsync();
    }

    public async Task ChangeAccountStatusAsync(Guid userId, bool status)
    {
        var user = await _users.SingleOrDefaultAsync(x => x.UserId == userId);
        user.IsActive = status;

        _users.Update(user);
        await _dbContext.SaveChangesAsync();
    }

    public async Task ChangeUserPassword(Guid userId, string password)
    {
        var user = await _users.SingleOrDefaultAsync(x => x.UserId == userId);
        user.Password = password;
        
        _users.Update(user);
        await _dbContext.SaveChangesAsync();
    }
}