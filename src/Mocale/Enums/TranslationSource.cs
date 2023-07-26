namespace Mocale.Enums;

/// <summary>
/// Translation Source
/// </summary>
public enum TranslationSource
{
    /// <summary>
    /// Translations came from internal provider (locally).
    /// </summary>
    Internal = 0,

    /// <summary>
    /// Translations came from external provider.
    /// </summary>
    External = 1,

    /// <summary>
    /// Translations came from cache, and the cache is up to date.
    /// </summary>
    WarmCache = 2,

    /// <summary>
    /// Translations came from cache, and the cache is expired (can be updated).
    /// </summary>
    ColdCache = 4,
}
