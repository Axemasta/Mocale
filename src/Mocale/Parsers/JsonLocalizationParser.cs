using System.Text.Json;

namespace Mocale.Parsers;

internal class JsonLocalizationParser : ILocalizationParser
{
    public Dictionary<string, string>? ParseLocalizationStream(Stream resourceStream)
    {
        return JsonSerializer.Deserialize<Dictionary<string, string>>(resourceStream);
    }

    public Dictionary<string, string>? ParseLocalizationString(string resourceString)
    {
        return JsonSerializer.Deserialize<Dictionary<string, string>>(resourceString);
    }
}

