using CertiBlock.Services.Polygon.Core.DTO;
using CertiBlock.Services.Polygon.Core.Entities;
using CertiBlock.Services.Polygon.Core.Repositories;
using CertiBlock.Shared.Mongo;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace CertiBlock.Services.Polygon.Infrastructure.DAL.Repositories;

public class PolygonMetricRepository : IPolygonMetricRepository
{
    private readonly IMongoCollection<PolygonMetrics> _collection;
    
    public PolygonMetricRepository(IOptions<MongoDbOptions> settings, IMongoClient client)
    {
        var db = client.GetDatabase(settings.Value.Database);
        _collection = db.GetCollection<PolygonMetrics>("polygon_metrics");
    }
    
    public async Task SavePolygonMetricsAsync(PolygonMetrics polygonMetrics)
    {
        await _collection.InsertOneAsync(polygonMetrics);
    }

    public async Task<PolygonMetrics> GetTransactionMetricsByCertificateAsync(Guid certificateId)
    {
        var filter = Builders<PolygonMetrics>.Filter.Eq(x => x.CertificateId, certificateId);
        return await _collection.Find(filter).FirstOrDefaultAsync();
    }

    public async Task DeleteTransactionMetricsByCertificateIdAsync(Guid certificateId)
    {
        var filter = Builders<PolygonMetrics>.Filter.Eq(c => c.CertificateId, certificateId);
        await _collection.DeleteOneAsync(filter);
    }

    public async Task<IEnumerable<Guid>> GetAllCertificateIdsWithMetricsAsync()
    {
        return await _collection
            .Distinct(x => x.CertificateId, _ => true)
            .ToListAsync();
    }

    public async Task<IEnumerable<PolygonMetrics>> GetUnfinalizedMetricsAsync()
    {
        var filter = Builders<PolygonMetrics>.Filter.Eq(x => x.IsFinalized, false);
        return await _collection.Find(filter).ToListAsync();
    }

    public async Task UpdatePolygonMetricsAsync(PolygonMetrics metrics)
    {
        var filter = Builders<PolygonMetrics>.Filter.Eq(x => x.Id, metrics.Id);
        await _collection.ReplaceOneAsync(filter, metrics);
    }

    public async Task<IEnumerable<PolygonMetricResearchDto>> GetAllResearchMetricsAsync()
    {
        return await _collection.Find(_ => true)
            .Project(x => new PolygonMetricResearchDto
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