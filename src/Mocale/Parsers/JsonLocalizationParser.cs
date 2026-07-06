using System.Text.Json;
using Ardalis.GuardClauses;

namespace Mocale.Parsers;

internal partial class JsonLocalizationParser(ILogger<JsonLocalizationParser> logger) : ILocalizationParser
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
            LogAnExceptionOccurredParsingLocalizationStream(ex);
            return null;
        }
    }

    #region Logging

    [LoggerMessage(LogLevel.Error, "An exception occurred parsing localization stream")]
    partial void LogAnExceptionOccurredParsingLocalizationStream(Exception exception);

    #endregion Logging
}
