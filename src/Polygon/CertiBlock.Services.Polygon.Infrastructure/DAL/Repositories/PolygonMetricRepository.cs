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
        var certificateIds = await _collection
            .Find(_ => true)
            .Project(x => x.CertificateId)
            .ToListAsync();
    
        return certificateIds.Distinct();
    }
}