using CertiBlock.Services.Polygon.Core.Entities;
using CertiBlock.Shared.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace CertiBlock.Services.Polygon.Infrastructure.Configurations;

public static class PolygonMetricsConfiguration
{
    public static void Configure()
    {
        BsonClassMap.RegisterClassMap<PolygonMetrics>(cm =>
        {
            cm.AutoMap();
            
            cm.MapIdProperty(x => x.Id)
                .SetSerializer(new GuidSerializer(BsonType.String));
            
            cm.MapProperty(x => x.CertificateId)
                .SetSerializer(new GuidSerializer(BsonType.String));
            
            cm.MapProperty(x => x.Blockchain)
                .SetSerializer(new EnumSerializer<Blockchain>(BsonType.String));
            
            cm.MapProperty(x => x.Operation)
                .SetSerializer(new EnumSerializer<Operation>(BsonType.String));
            
            cm.MapProperty(x => x.TransactionCostNative)
                .SetSerializer(new DecimalSerializer(BsonType.Decimal128));

            cm.MapProperty(x => x.TransactionCostUsd)
                .SetSerializer(new DecimalSerializer(BsonType.Decimal128));
        });
    }
}