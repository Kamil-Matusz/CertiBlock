using CertiBlock.Services.Ethereum.Core.DTO;
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
        return await _collection
            .Distinct(x => x.CertificateId, _ => true)
            .ToListAsync();
    }

    public async Task<IEnumerable<EthereumMetrics>> GetUnfinalizedMetricsAsync()
    {
        var filter = Builders<EthereumMetrics>.Filter.Eq(x => x.IsFinalized, false);
        return await _collection.Find(filter).ToListAsync();
    }

    public async Task UpdateEthereumMetricsAsync(EthereumMetrics metrics)
    {
        var filter = Builders<EthereumMetrics>.Filter.Eq(x => x.Id, metrics.Id);
        await _collection.ReplaceOneAsync(filter, metrics);
    }

    public async Task<IEnumerable<EthereumMetricResearchDto>> GetAllResearchMetricsAsync()
    {
        return await _collection.Find(_ => true)
            .Project(x => new EthereumMetricResearchDto
            {
                Id = x.Id,
                CertificateId = x.CertificateId,
                Blockchain = x.Blockchain,
                Operation = x.Operation,
                TransactionHash = x.TransactionHash,
                DataSizeBytes = x.DataSizeBytes,
                Confirmations = x.Confirmations,
                GasUsed = x.GasUsed,
                TransactionCostUsd = x.TransactionCostUsd,
                InclusionTimeSeconds = x.InclusionTimeSeconds,
                BlockNumber = x.BlockNumber,
                FinalizationTimeSeconds = x.FinalizationTimeSeconds
            })
            .ToListAsync();
    }
}