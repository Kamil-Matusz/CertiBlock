using CertiBlock.Services.Ethereum.Core.Entities;
using CertiBlock.Services.Ethereum.Core.Repositories;
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
}