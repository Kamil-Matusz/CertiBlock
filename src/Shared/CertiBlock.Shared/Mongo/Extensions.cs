using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace CertiBlock.Shared.Mongo;

public static class Extensions
{
    private const string MongoSectionName = "Mongo";

    public static IServiceCollection AddMongo(this IServiceCollection services, IConfiguration configuration)
    {
        var section = configuration.GetSection(MongoSectionName);
        services.Configure<MongoDbOptions>(section);
        var options = configuration.GetOptions<MongoDbOptions>(MongoSectionName);

        var client = new MongoClient(options.ConnectionString);
        var database = client.GetDatabase(options.Database);

        services.AddSingleton<IMongoClient>(client);
        services.AddSingleton(database);

        return services;
    }
    
    private static T GetOptions<T>(this IConfiguration configuration, string sectionName) where T : class, new()
    {
        var options = new T();
        var section = configuration.GetSection(sectionName);
        section.Bind(options);

        return options;
    }
}