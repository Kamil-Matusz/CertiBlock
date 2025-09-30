using CertiBlock.Services.Ethereum.Core.Entities;
using CertiBlock.Shared.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;

namespace CertiBlock.Services.Ethereum.Infrastructure.Configurations;

public static class EthereumMetricsConfiguration
{
    public static void Configure()
    {
        BsonClassMap.RegisterClassMap<EthereumMetrics>(cm =>
        {
            cm.AutoMap();
            
            cm.MapIdProperty(x => x.Id)
                .SetSerializer(new MongoDB.Bson.Serialization.Serializers.GuidSerializer(BsonType.String));
            
            cm.MapProperty(x => x.CertificateId)
                .SetSerializer(new MongoDB.Bson.Serialization.Serializers.GuidSerializer(BsonType.String));
            
            cm.MapProperty(x => x.Blockchain)
                .SetSerializer(new MongoDB.Bson.Serialization.Serializers.EnumSerializer<Blockchain>(BsonType.String));
            
            cm.MapProperty(x => x.Operation)
                .SetSerializer(new MongoDB.Bson.Serialization.Serializers.EnumSerializer<Operation>(BsonType.String));
        });
    }
}