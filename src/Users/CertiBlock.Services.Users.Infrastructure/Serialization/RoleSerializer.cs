using CertiBlock.Services.Users.Core.ValueObjects;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace CertiBlock.Services.Users.Infrastructure.Serialization;

public class RoleSerializer : SerializerBase<Role>
{
    public override Role Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
    {
        var value = context.Reader.ReadString();
        return new Role(value);
    }

    public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, Role value)
    {
        context.Writer.WriteString(value?.Value);
    }
}