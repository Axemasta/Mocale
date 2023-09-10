using System.Resources;

namespace Mocale.Parsers;
internal class ResxLocalizationParser : ILocalizationParser
{
    public Dictionary<string, string>? ParseLocalizationStream(Stream resourceStream)
    {
        var reader = new ResourceReader(resourceStream);

        //return resourceSet.Cast<DictionaryEntry>().ToDictionary(r => r.Key.ToString(), r => r.Value.ToString())
        //    ?? new Dictionary<string, string>();

        throw new NotImplementedException();
    }

    public Dictionary<string, string>? ParseLocalizationString(string resourceString)
    {
        throw new NotImplementedException();
    }
}

