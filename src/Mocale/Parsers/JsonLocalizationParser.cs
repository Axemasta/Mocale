using System.Text.Json;
using Ardalis.GuardClauses;

namespace Mocale.Parsers;

internal class JsonLocalizationParser(ILogger<JsonLocalizationParser> logger) : ILocalizationParser
{
    private readonly ILogger logger = Guard.Against.Null(logger, nameof(logger));

    public Dictionary<string, string>? ParseLocalizationStream(Stream resourceStream)
    {
        try
        {
            return JsonSerializer.Deserialize<Dictionary<string, string>>(resourceStream);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An exception occurred parsing localization stream");
            return null;
        }
    }
}

