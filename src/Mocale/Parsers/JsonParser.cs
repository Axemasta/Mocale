using System.Text.Json;
using Mocale.Abstractions;

namespace Mocale.Parsers;

internal class JsonParser : ILocalizationParser
{
    public Dictionary<string, string> ParseLocalizationStream(Stream resourceStream)
    {
        return JsonSerializer.Deserialize<Dictionary<string, string>>(resourceStream);
    }

    public Dictionary<string, string> ParseLocalizationString(string rawLocalizations)
    {
        return JsonSerializer.Deserialize<Dictionary<string, string>>(rawLocalizations);
    }
}
