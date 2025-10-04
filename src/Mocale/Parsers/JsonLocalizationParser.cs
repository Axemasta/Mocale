using System.Text.Json;
using Ardalis.GuardClauses;
using Mocale.Serialization;

namespace Mocale.Parsers;

internal class JsonLocalizationParser(ILogger<JsonLocalizationParser> logger) : ILocalizationParser
{
    private readonly ILogger logger = Guard.Against.Null(logger, nameof(logger));

    public Dictionary<string, string>? ParseLocalizationStream(Stream resourceStream)
    {
        try
        {
            return JsonSerializer.Deserialize(resourceStream, LocalizationJsonContext.Default.DictionaryStringString);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An exception occurred parsing localization stream");
            return null;
        }
    }
}

