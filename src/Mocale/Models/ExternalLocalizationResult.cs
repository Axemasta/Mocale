namespace Mocale.Models;

internal class ExternalLocalizationResult : IExternalLocalizationResult
{
    public required bool Success { get; set; }

    public required Dictionary<string, string> Localizations { get; set; }
}
