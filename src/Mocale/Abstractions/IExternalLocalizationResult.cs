namespace Mocale.Abstractions;

public interface IExternalLocalizationResult
{
    bool Succeess { get; }

    Dictionary<string, string> Localizations { get; }
}
