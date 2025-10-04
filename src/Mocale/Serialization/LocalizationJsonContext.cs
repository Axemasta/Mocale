using System.Text.Json.Serialization;

namespace Mocale.Serialization;

[JsonSerializable(typeof(Dictionary<string, string>))]
internal partial class LocalizationJsonContext : JsonSerializerContext
{
}
