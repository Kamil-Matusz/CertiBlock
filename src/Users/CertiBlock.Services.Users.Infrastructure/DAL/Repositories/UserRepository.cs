using CertiBlock.Services.Users.Core.Entities;
using CertiBlock.Services.Users.Core.Exceptions;
using CertiBlock.Services.Users.Core.Repositories;
using CertiBlock.Services.Users.Core.ValueObjects;
using CertiBlock.Services.Users.Infrastructure.DAL.PostgreSQL;
using Microsoft.EntityFrameworkCore;
using Npgsql;

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
        try
        {
            await _dbContext.SaveChangesAsync();
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation })
        {
            throw new EmailAlreadyInUseException(user.Email);
        }
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
        var user = await GetRequiredUserAsync(userId);
        user.Role = role;

        await _dbContext.SaveChangesAsync();
    }

    public async Task ChangeAccountStatusAsync(Guid userId, bool status)
    {
        var user = await GetRequiredUserAsync(userId);
        user.IsActive = status;

        await _dbContext.SaveChangesAsync();
    }

    public async Task ChangeUserPassword(Guid userId, string password)
    {
        var user = await GetRequiredUserAsync(userId);
        user.Password = password;

        await _dbContext.SaveChangesAsync();
    }

    private async Task<User> GetRequiredUserAsync(Guid userId)
        => await _users.SingleOrDefaultAsync(x => x.UserId == userId) ?? throw new UserNotFoundException(userId);
}