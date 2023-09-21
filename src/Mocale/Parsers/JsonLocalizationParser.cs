using System.Text.Json;
using Ardalis.GuardClauses;

namespace Mocale.Parsers;

internal class JsonLocalizationParser : ILocalizationParser
{
    private readonly ILogger logger;

    public JsonLocalizationParser(ILogger<JsonLocalizationParser> logger)
    {
        this.logger = Guard.Against.Null(logger, nameof(logger));
    }

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

