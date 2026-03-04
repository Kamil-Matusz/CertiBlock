using CertiBlock.Services.Users.Core.Entities;
using CertiBlock.Services.Users.Core.ValueObjects;
using CertiBlock.Services.Users.Application.Security;
using Microsoft.EntityFrameworkCore;

namespace CertiBlock.Services.Users.Infrastructure.DAL.PostgreSQL.Seeders;

internal sealed class UserSeeder(UsersDbContext dbContext, IPasswordManager passwordManager)
{
    public async Task SeedAsync()
    {
        if (await dbContext.Users.AnyAsync())
        {
            return;
        }

        var users = GetUsers();
        await dbContext.Users.AddRangeAsync(users);
        await dbContext.SaveChangesAsync();
    }

    private IEnumerable<User> GetUsers()
    {
        return new List<User>
        {
            new User(
                userId: Guid.NewGuid(),
                email: "admin@admin.com",
                password: passwordManager.Secure("password"),
                role: Role.Admin(),
                isActive: true,
                createdAt: DateTime.UtcNow
            ),
            new User(
                userId: Guid.NewGuid(),
                email: "user@certiblock.com",
                password: passwordManager.Secure("password"),
                role: Role.User(),
                isActive: true,
                createdAt: DateTime.UtcNow
            )
        };
    }
}
