using CertiBlock.Shared.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CertiBlock.Services.Polygon.Core.Entities;

public class PolygonMetrics
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public Guid Id { get; set; } = Guid.NewGuid();

    [BsonRepresentation(BsonType.String)]
    public Guid CertificateId { get; set; }

    [BsonRepresentation(BsonType.String)]
    public Blockchain Blockchain { get; set; }

    [BsonRepresentation(BsonType.String)]
    public Operation Operation { get; set; }

    public string TransactionHash { get; set; }

    public int DataSizeBytes { get; set; }

    public int Confirmations { get; set; }

    public decimal TransactionCostUsd { get; set; }

    public decimal TransactionCostNative { get; set; }

    public long GasUsed { get; set; }

    public double GasUtilizationRatio { get; set; }
}