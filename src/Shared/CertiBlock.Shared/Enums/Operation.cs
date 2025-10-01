using System.Text.Json.Serialization;

namespace CertiBlock.Shared.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum Operation
{
    Verify,
    Register
}