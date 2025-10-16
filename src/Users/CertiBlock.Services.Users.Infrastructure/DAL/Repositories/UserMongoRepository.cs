using CertiBlock.Services.Users.Core.Entities;
using CertiBlock.Services.Users.Core.Repositories;
using CertiBlock.Services.Users.Core.ValueObjects;
using CertiBlock.Shared.Mongo;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace CertiBlock.Services.Users.Infrastructure.DAL.Repositories;

internal sealed class UserMongoRepository : IUserRepository
{
    private readonly IMongoCollection<User> _collection;

    public UserMongoRepository(IOptions<MongoDbOptions> settings, IMongoClient client)
    {
        var db = client.GetDatabase(settings.Value.Database);
        _collection = db.GetCollection<User>("users");
    }
    
    public Task<User> GetUserByIdAsync(Guid userId) =>
        _collection.Find(x => x.UserId == userId).SingleOrDefaultAsync();

    public Task<User> GetUserByEmailAsync(string email) =>
        _collection.Find(x => x.Email == email).SingleOrDefaultAsync();

    public Task AddUserAsync(User user) =>
        _collection.InsertOneAsync(user);

    public async Task<bool> CheckAccountActivity(string email)
    {
        var projection = Builders<User>.Projection.Expression(x => x.IsActive);
        
        var isActive = await _collection
            .Find(x => x.Email == email)
            .Project(projection)
            .FirstOrDefaultAsync();

        return isActive;
    }

    public Task DeleteUserAsync(User user) =>
        _collection.DeleteOneAsync(x => x.UserId == user.UserId);

    public async Task ChangeUserRoleAsync(Guid userId, Role role)
    {
        var update = Builders<User>.Update.Set(x => x.Role, role);
        await _collection.UpdateOneAsync(x => x.UserId == userId, update);
    }

    public async Task ChangeAccountStatusAsync(Guid userId, bool status)
    {
        var update = Builders<User>.Update.Set(x => x.IsActive, status);
        await _collection.UpdateOneAsync(x => x.UserId == userId, update);
    }

    public async Task ChangeUserPassword(Guid userId, string password)
    {
        var update = Builders<User>.Update.Set(x => x.Password, password);
        await _collection.UpdateOneAsync(x => x.UserId == userId, update);
    }
}