using CertiBlock.Shared.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CertiBlock.Services.Ethereum.Core.Entities;

public class BlockchainTransaction
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public Guid Id { get; set; } = Guid.NewGuid();

    [BsonRepresentation(BsonType.String)]
    public Guid CertificateId { get; set; }
    public string CertificateHash { get; set; }
    public string Issuer { get; set; }

    [BsonRepresentation(BsonType.String)]
    public Blockchain Blockchain { get; set; }
    public string TransactionHash { get; set; }
    
    [BsonRepresentation(BsonType.String)]
    public Status Status { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime? UpdatedAt { get; set; }
}