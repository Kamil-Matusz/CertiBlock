using CertiBlock.Services.Users.Core.Entities;
using CertiBlock.Services.Users.Infrastructure.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace CertiBlock.Services.Users.Infrastructure.DAL.Mongo;

public static class UserConfiguration
{
    public static void Configure()
    {
        BsonClassMap.RegisterClassMap<User>(cm =>
        {
            cm.AutoMap();
            
            cm.MapIdProperty(x => x.UserId)
                .SetSerializer(new GuidSerializer(BsonType.String));
            
            cm.MapProperty(x => x.Email)
                .SetIsRequired(true);
            
            cm.MapProperty(x => x.Password)
                .SetIsRequired(true);
            
            cm.MapProperty(x => x.Role)
                .SetSerializer(new RoleSerializer())
                .SetIsRequired(true);
            
            cm.MapProperty(x => x.IsActive)
                .SetIsRequired(true);
            
            cm.MapProperty(x => x.CreatedAt)
                .SetIsRequired(true)
                .SetSerializer(new DateTimeSerializer(DateTimeKind.Utc));
        });
    }
}