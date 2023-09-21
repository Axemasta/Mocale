namespace Mocale.Abstractions;

internal interface ILocalizationParser
{
    Dictionary<string, string>? ParseLocalizationStream(Stream resourceStream);
}
