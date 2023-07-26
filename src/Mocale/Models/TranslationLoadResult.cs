namespace Mocale.Models;

public class TranslationLoadResult
{
    public bool Loaded { get; set; }

    public TranslationSource Source { get; set; }

    public Dictionary<string, string> Translations { get; set; } = new Dictionary<string, string>();
}
