using CertiBlock.Services.Polygon.Core.Entities;
using CertiBlock.Shared.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;

namespace CertiBlock.Services.Polygon.Infrastructure.Configurations;

public static class BlockchainTransactionConfiguration
{
    public static void Configure()
    {
        BsonClassMap.RegisterClassMap<BlockchainTransaction>(cm =>
        {
            cm.AutoMap();
            
            cm.MapIdProperty(x => x.Id)
                .SetSerializer(new MongoDB.Bson.Serialization.Serializers.GuidSerializer(BsonType.String));
            
            cm.MapProperty(x => x.CertificateId)
                .SetSerializer(new MongoDB.Bson.Serialization.Serializers.GuidSerializer(BsonType.String));
            
            cm.MapProperty(x => x.Blockchain)
                .SetSerializer(new MongoDB.Bson.Serialization.Serializers.EnumSerializer<Blockchain>(BsonType.String));
            
            cm.MapProperty(x => x.Status)
                .SetSerializer(new MongoDB.Bson.Serialization.Serializers.EnumSerializer<Status>(BsonType.String));
        });
    }
}