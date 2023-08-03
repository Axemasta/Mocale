namespace Mocale.Models;

public class TranslationLoadResult
{
    public bool Loaded { get; set; }

    public TranslationSource Source { get; set; }

    public required Localization Localization { get; set; }
}
