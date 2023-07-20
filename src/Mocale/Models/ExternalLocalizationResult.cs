namespace Mocale.Models;

internal class ExternalLocalizationResult : IExternalLocalizationResult
{
    public bool Succeess { get; set; }

    public Dictionary<string, string> Localizations { get; set; }
}
