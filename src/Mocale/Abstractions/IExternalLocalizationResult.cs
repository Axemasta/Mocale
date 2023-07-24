namespace Mocale.Abstractions;

public interface IExternalLocalizationResult
{
    bool Success { get; }

    Dictionary<string, string> Localizations { get; }
}
