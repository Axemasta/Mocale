using System.Resources.NetStandard;
using Ardalis.GuardClauses;

namespace Mocale.Parsers;

internal class ResxLocalizationParser : ILocalizationParser
{
    private readonly ILogger<ResxLocalizationParser> logger;

    public ResxLocalizationParser(ILogger<ResxLocalizationParser> logger)
    {
        this.logger = Guard.Against.Null(logger, nameof(logger));
    }

    public Dictionary<string, string>? ParseLocalizationStream(Stream resourceStream)
    {
        try
        {
            var reader = new ResXResourceReader(resourceStream);

            var localizations = new Dictionary<string, string>();

            var enumerator = reader.GetEnumerator();

            while (enumerator.MoveNext())
            {
                var key = (string)enumerator.Key;
                var value = enumerator.Value as string ?? string.Empty;

                localizations.Add(key, value);
            }

            return localizations;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An exception occurred parsing localization stream");
            return null;
        }
    }
}

