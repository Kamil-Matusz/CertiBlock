using CertiBlock.Services.Users.Infrastructure.DAL.PostgreSQL.Seeders;
using CertiBlock.Services.Users.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace CertiBlock.Services.Users.Infrastructure.DAL.PostgreSQL;

internal sealed class DatabaseInitializer(IServiceProvider serviceProvider) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        // auto migrations
        using (var scope = serviceProvider.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<UsersDbContext>();
            await dbContext.Database.MigrateAsync(cancellationToken);
            
            // seed data
            var passwordManager = scope.ServiceProvider.GetRequiredService<PasswordManager>();
            var userSeeder = new UserSeeder(dbContext, passwordManager);
            await userSeeder.SeedAsync();
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}