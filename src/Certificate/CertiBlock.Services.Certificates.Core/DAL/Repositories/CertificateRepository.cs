using CertiBlock.Services.Certificates.Core.Entities;
using CertiBlock.Shared.Mongo;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace CertiBlock.Services.Certificates.Core.DAL.Repositories;

public class CertificateRepository : ICertificateRepository
{
    private readonly IMongoCollection<Certificate> _collection;

    public CertificateRepository(IOptions<MongoDbOptions> settings, IMongoClient client)
    {
        var db = client.GetDatabase(settings.Value.Database);
        _collection = db.GetCollection<Certificate>("certificates");
    }
    
    public async Task SaveCertificateAsync(Certificate certificate)
    {
        await _collection.InsertOneAsync(certificate);
    }

    public async Task<Certificate> GetCertificateByIdAsync(Guid id)
    {
        var filter = Builders<Certificate>.Filter.Eq(x => x.Id, id);
        return await _collection.Find(filter).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<Certificate>> GetAllCertificatesAsync()
    {
        return await _collection.Find(_ => true).ToListAsync();
    }

    public async Task<IEnumerable<Certificate>> GetCertificateByIssuerIdAsync(string issuerId)
    {
        var filter = Builders<Certificate>.Filter.Eq(x => x.IssuerId, issuerId);
        return await _collection.Find(filter).ToListAsync();
    }

    public async Task DeleteCertificateAsync(Guid id)
    {
        var filter = Builders<Certificate>.Filter.Eq(c => c.Id, id);
        await _collection.DeleteOneAsync(filter);
    }

    public async Task<IEnumerable<Certificate>> GetCertificatedPagedAsync(int page, int pageSize)
    {
        return await _collection.Find(_ => true)
            .Skip((page - 1) * pageSize)
            .Limit(pageSize)
            .ToListAsync();
    }

    public async Task<long> GetCertificateCountAsync()
    {
        return await _collection.CountDocumentsAsync(_ => true);
    }
}