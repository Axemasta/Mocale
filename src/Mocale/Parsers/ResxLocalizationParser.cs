using System.Resources;
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
        var reader = new ResourceReader(resourceStream);

        //return resourceSet.Cast<DictionaryEntry>().ToDictionary(r => r.Key.ToString(), r => r.Value.ToString())
        //    ?? new Dictionary<string, string>();

        throw new NotImplementedException();
    }
}

