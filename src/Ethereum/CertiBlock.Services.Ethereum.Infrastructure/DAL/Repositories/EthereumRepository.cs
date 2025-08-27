using CertiBlock.Services.Ethereum.Core.Entities;
using CertiBlock.Services.Ethereum.Core.Repositories;
using CertiBlock.Shared.Enums;
using CertiBlock.Shared.Mongo;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace CertiBlock.Services.Ethereum.Infrastructure.DAL.Repositories;

public class EthereumRepository : IEthereumRepository
{
    private readonly IMongoCollection<BlockchainTransaction> _collection;
    
    public EthereumRepository(IOptions<MongoDbOptions> settings, IMongoClient client)
    {
        var db = client.GetDatabase(settings.Value.Database);
        _collection = db.GetCollection<BlockchainTransaction>("ethereum");
    }


    public async Task<BlockchainTransaction> GetBlockchainTransactionByIdAsync(Guid id)
    {
        var filter = Builders<BlockchainTransaction>.Filter.Eq(x => x.Id, id);
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
    
    public async Task<bool> TransactionExistsAsync(string transactionHash)
    {
        var filter = Builders<BlockchainTransaction>.Filter.Eq(x => x.TransactionHash, transactionHash);
        var count = await _collection.CountDocumentsAsync(filter);
        return count > 0;
    }
    
    public async Task<IEnumerable<BlockchainTransaction>> GetTransactionsPagedAsync(int page, int pageSize)
    {
        return await _collection.Find(_ => true)
            .Skip((page - 1) * pageSize)
            .Limit(pageSize)
            .ToListAsync();
    }
    
    public async Task<IEnumerable<BlockchainTransaction>> GetFailedTransactionsAsync()
    {
        var filter = Builders<BlockchainTransaction>.Filter.Eq(x => x.Status, Status.Failed);
        return await _collection.Find(filter).ToListAsync();
    }
    
    public async Task<long> GetTransactionCountAsync()
    {
        return await _collection.CountDocumentsAsync(_ => true);
    }
}