namespace Mocale.Abstractions;

/// <summary>
/// Translation Updater
/// </summary>
internal interface ITranslationUpdater
{
    /// <summary>
    /// Update Translations From Source
    /// </summary>
    /// <param name="localization"></param>
    /// <param name="source"></param>
    /// <param name="notify">Whether this should notify observers</param>
    void UpdateTranslations(Localization localization, TranslationSource source, bool notify = true);
}
