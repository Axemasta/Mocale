namespace Mocale.Abstractions;

internal interface ILocalizationParser
{
    Dictionary<string, string> ParseLocalizationString(string resourceString);

    Dictionary<string, string> ParseLocalizationStream(Stream resourceStream);
}
