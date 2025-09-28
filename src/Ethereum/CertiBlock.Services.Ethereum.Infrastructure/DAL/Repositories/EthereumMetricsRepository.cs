using CertiBlock.Services.Ethereum.Core.Entities;
using CertiBlock.Services.Ethereum.Core.Repositories;
using CertiBlock.Shared.Mongo;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace CertiBlock.Services.Ethereum.Infrastructure.DAL.Repositories;

public class EthereumMetricsRepository : IEthereumMetricRepository
{
    private readonly IMongoCollection<EthereumMetrics> _collection;
    
    public EthereumMetricsRepository(IOptions<MongoDbOptions> settings, IMongoClient client)
    {
        var db = client.GetDatabase(settings.Value.Database);
        _collection = db.GetCollection<EthereumMetrics>("ethereum_metrics");
    }
    
    public async Task SaveEthereumMetricsAsync(EthereumMetrics ethereumMetrics)
    {
        await _collection.InsertOneAsync(ethereumMetrics);
    }
}