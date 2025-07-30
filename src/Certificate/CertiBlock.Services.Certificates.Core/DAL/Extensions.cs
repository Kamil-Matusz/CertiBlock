using CertiBlock.Services.Certificates.Core.DAL.MongoDB;
using CertiBlock.Services.Certificates.Core.DAL.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace CertiBlock.Services.Certificates.Core.DAL;

public static class Extensions
{
    private const string MongoSectionName = "mongo";

    public static IServiceCollection AddMongo(this IServiceCollection services, IConfiguration configuration)
    {
        var section = configuration.GetSection(MongoSectionName);
        services.Configure<MongoDBOptions>(section);
        var options = configuration.GetOptions<MongoDBOptions>(MongoSectionName);

        var client = new MongoClient(options.ConnectionString);
        var database = client.GetDatabase(options.Database);

        services.AddSingleton<IMongoClient>(client);
        services.AddSingleton(database);
        
        // Repositories
        services.AddScoped<ICertificateRepository, CertificateRepository>();

        return services;
    }

    public static T GetOptions<T>(this IConfiguration configuration, string sectionName) where T : class, new()
    {
        var options = new T();
        var section = configuration.GetSection(sectionName);
        section.Bind(options);

        return options;
    }
}