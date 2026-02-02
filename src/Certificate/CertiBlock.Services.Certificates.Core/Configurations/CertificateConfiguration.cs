using CertiBlock.Services.Certificates.Core.Entities;
using CertiBlock.Shared.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;

namespace CertiBlock.Services.Certificates.Core.Configurations;

public static class CertificateConfiguration
{
    public static void Configure()
    {
        BsonClassMap.RegisterClassMap<Certificate>(cm =>
        {
            cm.AutoMap();
            
            cm.MapIdProperty(x => x.Id)
                .SetSerializer(new MongoDB.Bson.Serialization.Serializers.GuidSerializer(BsonType.String));
            
            cm.MapProperty(x => x.Blockchain)
                .SetSerializer(new MongoDB.Bson.Serialization.Serializers.EnumSerializer<Blockchain>(BsonType.String));
        });
    }
}