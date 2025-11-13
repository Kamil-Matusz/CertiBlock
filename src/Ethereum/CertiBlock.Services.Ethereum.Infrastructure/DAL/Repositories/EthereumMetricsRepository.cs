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

    public async Task<EthereumMetrics> GetTransactionMetricsByCertificateAsync(Guid certificateId)
    {
        var filter = Builders<EthereumMetrics>.Filter.Eq(x => x.CertificateId, certificateId);
        return await _collection.Find(filter).FirstOrDefaultAsync();
    }

    public async Task DeleteTransactionMetricsByCertificateIdAsync(Guid certificateId)
    {
        var filter = Builders<EthereumMetrics>.Filter.Eq(c => c.CertificateId, certificateId);
        await _collection.DeleteOneAsync(filter);
    }

    public async Task<IEnumerable<Guid>> GetAllCertificateIdsWithMetricsAsync()
    {
        var projection = Builders<EthereumMetrics>.Projection
            .Include(x => x.CertificateId)
            .Exclude(x => x.Id);
    
        var certificateIds = await _collection
            .Find(_ => true)
            .Project(x => x.CertificateId)
            .ToListAsync();
    
        return certificateIds.Distinct();
    }
}