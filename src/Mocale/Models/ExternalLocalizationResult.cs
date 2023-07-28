namespace Mocale.Models;

internal class ExternalLocalizationResult : IExternalLocalizationResult
{
    public required bool Success { get; set; }

    public Dictionary<string, string> Localizations { get; set; } = new Dictionary<string, string>();
}
