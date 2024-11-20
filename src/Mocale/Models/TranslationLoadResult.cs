namespace Mocale.Models;

/// <summary>
/// Translation Load Result
/// </summary>
public class TranslationLoadResult
{
    /// <summary>
    /// Whether translations were loaded
    /// </summary>
    public bool Loaded { get; set; }

    /// <summary>
    /// Source of the translations
    /// </summary>
    public TranslationSource Source { get; set; }

    /// <summary>
    ///
    /// </summary>
    public required Localization Localization { get; set; }
}
