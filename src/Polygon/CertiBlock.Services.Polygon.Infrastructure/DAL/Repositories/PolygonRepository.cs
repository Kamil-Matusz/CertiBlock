using CertiBlock.Services.Polygon.Core.Entities;
using CertiBlock.Services.Polygon.Core.Repositories;
using CertiBlock.Shared.Enums;
using CertiBlock.Shared.Mongo;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace CertiBlock.Services.Polygon.Infrastructure.DAL.Repositories;

public class PolygonRepository : IPolygonRepository
{
    private readonly IMongoCollection<BlockchainTransaction> _collection;
    
    public PolygonRepository(IOptions<MongoDbOptions> settings, IMongoClient client)
    {
        var db = client.GetDatabase(settings.Value.Database);
        _collection = db.GetCollection<BlockchainTransaction>("polygon");
    }
    
    public async Task<BlockchainTransaction> GetBlockchainTransactionByIdAsync(Guid id)
    {
        var filter = Builders<BlockchainTransaction>.Filter.Eq(x => x.Id, id);
        return await _collection.Find(filter).FirstOrDefaultAsync();
    }

    public async Task<BlockchainTransaction> GetBlockchainTransactionByCertificateIdAsync(Guid certificateId)
    {
        var filter = Builders<BlockchainTransaction>.Filter.Eq(x => x.CertificateId, certificateId);
        return await _collection.Find(filter).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<BlockchainTransaction>> GetAllBlockchainTransactionsAsync()
    {
        return await _collection.Find(_ => true).ToListAsync();
    }

    public async Task SaveBlockchainTransactionAsync(BlockchainTransaction blockchainTransaction)
    {
        await _collection.InsertOneAsync(blockchainTransaction);
    }

    public async Task DeleteBlockchainTransactionAsync(Guid id)
    {
        var filter = Builders<BlockchainTransaction>.Filter.Eq(c => c.Id, id);
        await _collection.DeleteOneAsync(filter);
    }

    public async Task UpdateBlockchainTransactionAsync(BlockchainTransaction transaction)
    {
        var filter = Builders<BlockchainTransaction>.Filter.Eq(x => x.Id, transaction.Id);
        await _collection.ReplaceOneAsync(filter, transaction);
    }
    
    public async Task<IEnumerable<BlockchainTransaction>> GetTransactionsPagedAsync(int page, int pageSize)
    {
        return await _collection.Find(_ => true)
            .Skip((page - 1) * pageSize)
            .Limit(pageSize)
            .ToListAsync();
    }

    public async Task<IEnumerable<BlockchainTransaction>> GetTransactionsByStatusAsync(Status status)
    {
        var filter = Builders<BlockchainTransaction>.Filter.Eq(t => t.Status, status);
        return await _collection.Find(filter).ToListAsync();
    }

    public async Task<IEnumerable<BlockchainTransaction>> GetTransactionsByStatusAsync(params Status[] statuses)
    {
        var filter = Builders<BlockchainTransaction>.Filter.In(t => t.Status, statuses);
        return await _collection.Find(filter).ToListAsync();
    }
    
    public async Task<long> GetTransactionCountAsync()
    {
        return await _collection.CountDocumentsAsync(_ => true);
    }

    public async Task<BlockchainTransaction?> GetByTransactionHashAsync(string transactionHash)
    {
        var filter = Builders<BlockchainTransaction>.Filter.Eq(x => x.TransactionHash, transactionHash);
        return await _collection.Find(filter).FirstOrDefaultAsync();
    }
}